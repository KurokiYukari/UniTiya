namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 将 DecoratedNode 执行指定的重复次数的 Decorator。
    /// </summary>
    public sealed class TimeRepeaterNode : DecoratorNode
    {
        public int RepeatTime { get; set; }
        int _currentTime = 0;

        public TimeRepeaterNode(BehaviourTreeNodeBase decoratedNode, int repeatTime = 1) : base(decoratedNode)
        {
            RepeatTime = repeatTime;
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            for (int i = _currentTime; i < RepeatTime; i++)
            {
                if (DecoratedNode.Update() == BehaviourTreeResult.Running)
                {
                    return BehaviourTreeResult.Running;
                }
                _currentTime++;
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
