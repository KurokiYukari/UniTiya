namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class RepeaterNode : DecoratorNode
    {
        public int RepeatTime { get; set; } = 1;
        int _currentTime = 0;

        public RepeaterNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(behaviourTree, decoratedNode)
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
                if (DecoratedNode.Update() == BehaviourTreeResult.Running)
                {
                    return NodeResult = BehaviourTreeResult.Running;
                }
                _currentTime++;
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
