using System.Collections.Generic;

namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SustainedNode : DecoratorNode
    {
        public float SustainedTime
        {
            get => _timerNode.TimeSpan;
            set => _timerNode.TimeSpan = value;
        }

        readonly SequenceNode _actualDecoratedNode;
        readonly SucceederNode _succeederNode;
        readonly TimerNode _timerNode;

        public SustainedNode(BehaviourTreeNodeBase decoratedNode, float sustainedTime) : base(decoratedNode)
        {
            _actualDecoratedNode = new SequenceNode();
            _actualDecoratedNode.AddChildNode(_succeederNode = new SucceederNode(DecoratedNode));
            _actualDecoratedNode.AddChildNode(_timerNode = new TimerNode(sustainedTime));
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            var result = _actualDecoratedNode.Update();

            if (result == BehaviourTreeResult.Running)
            {
                return BehaviourTreeResult.Running;
            }
            else
            {
                return DecoratedNode.NodeResult;
            }
        }

        protected internal override void Init(TiyaBehaviourTree behaviourTree)
        {
            base.Init(behaviourTree);

            _actualDecoratedNode.Init(behaviourTree);
            _succeederNode.Init(behaviourTree);
            _timerNode.Init(behaviourTree);
        }
    }
}
