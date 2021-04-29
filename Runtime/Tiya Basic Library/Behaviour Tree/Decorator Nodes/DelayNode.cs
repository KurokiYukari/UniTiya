namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class DelayNode : DecoratorNode
    {
        public float DelayTime
        {
            get => _timerNode.TimeSpan;
            set => _timerNode.TimeSpan = value;
        }

        readonly SequenceNode _actualDecoratedNode;
        readonly TimerNode _timerNode;

        public DelayNode(BehaviourTreeNodeBase decoratedNode, float delayTime) : base(decoratedNode)
        {
            _actualDecoratedNode = new SequenceNode();
            _actualDecoratedNode.AddChildNode(_timerNode = new TimerNode(delayTime));
            _actualDecoratedNode.AddChildNode(DecoratedNode);
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return _actualDecoratedNode.Update();
        }

        protected internal override void Init(TiyaBehaviourTree behaviourTree)
        {
            base.Init(behaviourTree);

            _actualDecoratedNode.Init(behaviourTree);
            _timerNode.Init(behaviourTree);
        }
    }
}
