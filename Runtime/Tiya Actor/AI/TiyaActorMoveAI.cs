using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using UniRx;

using Sarachan.UniTiya.Commands;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// 基于 <see cref="NavMeshAgent"/>，适用于 <see cref="IActorController"/> 的寻路
    /// 组件。它会向 actor 发送移动命令使 actor 尝试主动移动到 <see cref="Destination"/>
    /// 位置。
    /// <para>
    /// 对于 <see cref="OffMeshLink"/>，自动生成的 Link 会使用默认方法或指定实现了 
    /// <see cref="IActorNavMeshLinkTraverser"/> 的方法跨越 Link，手动设置的会尝
    /// 试在 Link 所在的 GameObject 上寻找实现了 <see cref="IActorNavMeshLinkTraverser"/> 
    /// 组件使用其提供的方法跨越 Link。
    /// </para>
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class TiyaActorMoveAI : MonoBehaviour
    {
        [TypeRestriction(typeof(IActorNavMeshLinkTraverser))]
        [SerializeField] Object _dropDownLinkTraverserObject;

        [TypeRestriction(typeof(IActorNavMeshLinkTraverser))]
        [SerializeField] Object _jumpAcrossLinkTraverserObject;

        IActorNavMeshLinkTraverser _dropDownTraverser;
        public IActorNavMeshLinkTraverser DropDownTraverser
        {
            get => _dropDownTraverser ??= _dropDownLinkTraverserObject?.ConvertTo<IActorNavMeshLinkTraverser>();
            set => _dropDownTraverser = value;
        }

        IActorNavMeshLinkTraverser _jumpAcrossTraverser;
        public IActorNavMeshLinkTraverser JumpAcrossTraverser
        {
            get => _jumpAcrossTraverser ??= _jumpAcrossLinkTraverserObject?.ConvertTo<IActorNavMeshLinkTraverser>();
            set => _jumpAcrossTraverser = value;
        }

        IActorController _actor;
        /// <summary>
        /// 寻路主体 Actor。
        /// </summary>
        public IActorController Actor => (_actor ??= GetComponent<IActorController>()) ?? throw new MissingComponentException();
        
        NavMeshAgent _navMeshAgent;
        /// <summary>
        /// 导航的 <see cref="NavMeshAgent"/>。
        /// </summary>
        public NavMeshAgent Agent => _navMeshAgent == null ? _navMeshAgent = GetComponent<NavMeshAgent>() : _navMeshAgent;

        [SerializeField] ActorMoveAIEvent _onArriveDestination;

        [System.Serializable]
        class ActorMoveAIEvent : UnityEvent<TiyaActorMoveAI> { }

        public event System.Action OnArriveDestination;

        public bool HasArrivedDestination { get; private set; } = false;
        bool _preHasArrivedDestination = false;

        /// <summary>
        /// 导航的目的地。设置之后该组件便会控制 <see cref="Actor"/> 将之导航至目的地。
        /// </summary>
        public Vector3 Destination
        {
            get => Agent.destination;
            set
            {
                var preValue = Agent.destination;
                Agent.destination = value;
                if (Agent.destination != preValue)
                {
                    HasArrivedDestination = false;
                    
                }
            }
        }

        protected void Start()
        {
            Agent.updatePosition = false;
            Agent.updateRotation = false;
            Agent.autoTraverseOffMeshLink = false;
            Agent.stoppingDistance = Agent.stoppingDistance > 0 ? Agent.stoppingDistance : 1;
        }

        protected void OnEnable()
        {
            Agent.enabled = true;

            Actor.OnMoving += AfterMoveListener;

            Agent.nextPosition = transform.position;
        }

        protected void OnDisable()
        {
            Agent.enabled = false;

            Actor.OnMoving -= AfterMoveListener;

            Actor.CommandProcessor.AddCommand(ActorCommands.Move(Vector3.zero));
        }

        void AfterMoveListener()
        {
            Agent.nextPosition = transform.position;
        }

        Object _linkOwner;
        public void CompleteOffMeshLink()
        {
            _linkOwner = null;
            Agent.CompleteOffMeshLink();
        }

        protected void Update()
        {
            // 处理 MeshLink
            if (Agent.isOnOffMeshLink)
            {
                switch (Agent.currentOffMeshLinkData.linkType)
                {
                    case OffMeshLinkType.LinkTypeManual:
                        if (_linkOwner != Agent.navMeshOwner)
                        {
                            _linkOwner = Agent.navMeshOwner;

                            var linkTraverser = Agent.navMeshOwner.ConvertTo<IActorNavMeshLinkTraverser>();
                            if (linkTraverser == null)
                            {
                                linkTraverser = DefaultManualLinkTraverser;
                                Debug.LogWarning($"Custom NavMeshLink {Agent.navMeshOwner.name} missing Component {nameof(IActorNavMeshLinkTraverser)}");
                            }
                            linkTraverser.TraverseNavMeshLink(this);
                        }
                        break;
                    case OffMeshLinkType.LinkTypeDropDown:
                        (DropDownTraverser ??= DefaultDropDownLinkTraverser).TraverseNavMeshLink(this);
                        break;
                    case OffMeshLinkType.LinkTypeJumpAcross:
                        (JumpAcrossTraverser ??= DefaultJumpAcrossLinkTraverser).TraverseNavMeshLink(this);
                        break;
                }
            }

            // 到达目的地事件
            _preHasArrivedDestination = HasArrivedDestination;
            if (!Agent.pathPending && Agent.remainingDistance < Agent.stoppingDistance)
            {
                HasArrivedDestination = true;
            }
            else
            {
                HasArrivedDestination = false;
            }
            if (!_preHasArrivedDestination && HasArrivedDestination)
            {
                OnArriveDestination?.Invoke();
                _onArriveDestination.Invoke(this);
            }

            // Actor 与 Agent 的同步
            // TODO: 直接在 IActorController 中开发 HorizontalSpeed ?
            Agent.speed = Actor.Velocity.magnitude;
        }

        public void MoveToNextPostion()
        {
            if (!HasArrivedDestination)
            {
                var direction = Agent.nextPosition - Actor.ActorTransform.position;
                if (direction.x == 0 && direction.z == 0)
                {
                    direction = Actor.ActorTransform.forward;
                }
                Actor.CommandProcessor.AddCommand(ActorCommands.Move(direction));
            }
        }

        #region Inner Types
        static IActorNavMeshLinkTraverser DefaultManualLinkTraverser { get; } = new ConfigurableLinkTraverser(DefaultFlashLinkTraverseAction);
        static IActorNavMeshLinkTraverser DefaultDropDownLinkTraverser { get; } = new ConfigurableLinkTraverser(DefaultDropDownLinkTraverseAction);
        static IActorNavMeshLinkTraverser DefaultJumpAcrossLinkTraverser { get; } = new ConfigurableLinkTraverser(DefaultFlashLinkTraverseAction);

        static void DefaultDropDownLinkTraverseAction(TiyaActorMoveAI actorMoveAI)
        {
            actorMoveAI.CompleteOffMeshLink();
        }

        static void DefaultFlashLinkTraverseAction(TiyaActorMoveAI actorMoveAI)
        {
            actorMoveAI.Actor.ActorTransform.position = actorMoveAI.Agent.currentOffMeshLinkData.endPos;

            var direction = actorMoveAI.Agent.currentOffMeshLinkData.endPos - actorMoveAI.Agent.currentOffMeshLinkData.startPos;
            actorMoveAI.Agent.velocity = Vector3.ProjectOnPlane(direction, Vector3.up);
            actorMoveAI.CompleteOffMeshLink();
        }
        #endregion
    }
}
