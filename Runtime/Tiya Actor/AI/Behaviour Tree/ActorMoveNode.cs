using UnityEngine;

using Sarachan.UniTiya.BehaviourTree;
using Sarachan.UniTiya.Commands;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    public class ActorMoveNode : ActorActionNodeBase
    {
        public float MoveSustainedTimeSpan
        {
            get => _actualNode.RepeatTimeSpan;
            set => _actualNode.RepeatTimeSpan = value;
        }

        public bool RandomDirection { get; set; } = true;

        public bool RelativeSpace { get; set; } = true;
        public Vector3 Direction { get; set; }

        readonly TimeSpanRepeaterNode _actualNode;

        public ActorMoveNode(float moveSustainedTimeSpan)
        {
            var moveNode = new SimpleActionNode(MoveUpdateListener);
            _actualNode = new TimeSpanRepeaterNode(moveNode, moveSustainedTimeSpan);
            _actualNode.OnReset += MoveResetListener;
        }

        void MoveResetListener()
        {
            if (RandomDirection)
            {
                Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }
        }

        void MoveUpdateListener()
        {
            Actor.CommandProcessor.AddCommand(
                ActorCommands.Move(RelativeSpace ? Actor.ActorTransform.TransformDirection(Direction) : Direction));
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return _actualNode.Update();
        }

        protected internal override void Init(TiyaBehaviourTree behaviourTree)
        {
            base.Init(behaviourTree);

            foreach (var node in _actualNode.NodesEnumerable)
            {
                node.Init(behaviourTree);
            }
        }
    }
}
