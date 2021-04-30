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
    /// ���� <see cref="NavMeshAgent"/>�������� <see cref="IActorController"/> ��Ѱ·
    /// ����������� actor �����ƶ�����ʹ actor ���������ƶ��� <see cref="Destination"/>
    /// λ�á�
    /// <para>
    /// ���� <see cref="OffMeshLink"/>���Զ����ɵ� Link ��ʹ��Ĭ�Ϸ�����ָ��ʵ���� 
    /// <see cref="IActorNavMeshLinkTraverser"/> �ķ�����Խ Link���ֶ����õĻ᳢
    /// ���� Link ���ڵ� GameObject ��Ѱ��ʵ���� <see cref="IActorNavMeshLinkTraverser"/> 
    /// ���ʹ�����ṩ�ķ�����Խ Link��
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
        /// Ѱ·���� Actor��
        /// </summary>
        public IActorController Actor => (_actor ??= GetComponent<IActorController>()) ?? throw new MissingComponentException();
        
        NavMeshAgent _navMeshAgent;
        /// <summary>
        /// ������ <see cref="NavMeshAgent"/>��
        /// </summary>
        public NavMeshAgent Agent => _navMeshAgent == null ? _navMeshAgent = GetComponent<NavMeshAgent>() : _navMeshAgent;

        [SerializeField] ActorMoveAIEvent _onArriveDestination;

        [System.Serializable]
        class ActorMoveAIEvent : UnityEvent<TiyaActorMoveAI> { }

        public event System.Action OnArriveDestination;

        public bool HasArrivedDestination { get; private set; } = false;
        bool _preHasArrivedDestination = false;

        /// <summary>
        /// ������Ŀ�ĵء�����֮������������ <see cref="Actor"/> ��֮������Ŀ�ĵء�
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
            // ���� MeshLink
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

            // ����Ŀ�ĵ��¼�
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

            // Actor �� Agent ��ͬ��
            // TODO: ֱ���� IActorController �п��� HorizontalSpeed ?
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
