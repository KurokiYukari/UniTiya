namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class RepeatUntilFailNode : DecoratorNode
    {
        public int RepeatTime { get; set; } = 1;
        int _currentTime = 0;

        public RepeatUntilFailNode(BehaviourTreeNodeBase decoratedNode) : base(decoratedNode)
        {
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            if (!(NodeResult == BehaviourTreeResult.Running))
            {
                Reset();
            }

            for (int i = _currentTime; i < RepeatTime; i++)
            {
                switch (DecoratedNode.Update())
                {
                    case BehaviourTreeResult.Success:
                        _currentTime++;
                        continue;
                    case BehaviourTreeResult.Failure:
                        return BehaviourTreeResult.Failure;
                    case BehaviourTreeResult.Running:
                        return BehaviourTreeResult.Running;
                }
            }

            return DecoratedNode.NodeResult;
        }

        protected internal override void Reset()
        {
            base.Reset();

            _currentTime = 0;
        }
    }
}
