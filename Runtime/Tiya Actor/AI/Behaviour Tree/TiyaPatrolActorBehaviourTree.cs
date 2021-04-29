using System.Linq;
using UnityEngine;

using Sarachan.UniTiya.BehaviourTree;
using Sarachan.UniTiya.Commands;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Actor/Tiya Patrol Actor Behaviour Tree")]
    public class TiyaPatrolActorBehaviourTree : TiyaBehaviourTree
    {
        [SerializeField] float _tryAttackDistance = 2f;

        [SerializeField] NodeWeightPair[] _randomInAttackingDistanceActions;

        BehaviourTreeNodeBase _rootNode;
        public override BehaviourTreeNodeBase RootNode => _rootNode;

        TiyaPatrolAI _patrolAI;
        public TiyaPatrolAI PatrolAI => _patrolAI ??= GetComponent<TiyaPatrolAI>();

        public TiyaActorMoveAI MoveAI => PatrolAI.ActorMoveAI;
        public IActorController Actor => MoveAI.Actor;

        protected void Awake()
        {
            InitBehaviourTree();
        }

        void InitBehaviourTree()
        {
            var rootSelector = new SelectorNode();
            _rootNode = rootSelector;

            rootSelector.AddChildNode(SequenceNode.CreateSimpleConditionalSequenceNode(
                () => !Actor.ActorView.IsLocked,
                () => {
                    PatrolAI.enabled = true;
                    MoveAI.MoveToNextPostion();

                    var target = Actor.ActorView.FindLockTargets().FirstOrDefault();
                    if (target != null)
                    {
                        Actor.CommandProcessor.AddCommand(ActorCommands.Lock(target));
                    }
                }));

            var lockSequence = new SequenceNode();
            rootSelector.AddChildNode(lockSequence);

            lockSequence.AddChildNode(new SimpleActionNode(() => PatrolAI.enabled = false));

            var distanceSelector = new SelectorNode();
            lockSequence.AddChildNode(distanceSelector);

            var inAttackingDistanceSequence = SequenceNode.CreateSimpleConditionalSequenceNode(
                () => Vector3.Distance(Actor.ActorTransform.position, Actor.ActorView.LockTarget.transform.position) < _tryAttackDistance,
                () => {
                    // TODO: 改为单纯的旋转？或者旋转也要添加条件？
                    //Actor.CommandProcessor.AddCommand(ActorCommands.Move(Actor.ActorView.LockTarget.transform.position - Actor.ActorTransform.position));
                });
            distanceSelector.AddChildNode(inAttackingDistanceSequence);

            var randomActionSelector = new RandomSelectorNode();
            inAttackingDistanceSequence.AddChildNode(randomActionSelector);

            // 攻击范围内自定义行为
            foreach (var pair in _randomInAttackingDistanceActions)
            {
                randomActionSelector.AddChildNode(pair.Node, pair.Weight);
            }

            distanceSelector.AddChildNode(new SimpleActionNode(
                () => {
                    MoveAI.Destination = Actor.ActorView.LockTarget.transform.position;
                    MoveAI.MoveToNextPostion();
                }));
        }
    }
}
