namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class RepeatUntilFailNode : DecoratorNode
    {
        public int RepeatTime { get; set; } = 1;
        int _currentTime = 0;

        public RepeatUntilFailNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(behaviourTree, decoratedNode)
        {
        }

        public override BehaviourTreeResult Update()
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
                        return NodeResult = BehaviourTreeResult.Failure;
                    case BehaviourTreeResult.Running:
                        return NodeResult = BehaviourTreeResult.Running;
                }
            }

            return NodeResult = DecoratedNode.NodeResult;
        }

        public override void Reset()
        {
            base.Reset();

            _currentTime = 0;
        }
    }
}
