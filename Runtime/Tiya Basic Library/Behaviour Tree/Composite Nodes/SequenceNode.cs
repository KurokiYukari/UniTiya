namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SequenceNode : CompositeNode
    {
        int _startIndex = 0;

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            for (int i = _startIndex; i < ChildNodes.Count; i++)
            {
                var node = ChildNodes[i];

                switch (node.Update())
                {
                    case BehaviourTreeResult.Success:
                        continue;
                    case BehaviourTreeResult.Failure:
                        _startIndex = 0;
                        return BehaviourTreeResult.Failure;
                    case BehaviourTreeResult.Running:
                        _startIndex = i;
                        return BehaviourTreeResult.Running;
                }
            }

            return BehaviourTreeResult.Success;
        }

        protected internal override void Reset()
        {
            base.Reset();

            _startIndex = 0;
        }

        public static CompositeNode CreateSimpleConditionalSequenceNode(
            System.Func<bool> conditionFunc, 
            System.Action action)
        {
            var sequence = new SequenceNode();
            sequence.AddChildNode(new ConditionalNode(conditionFunc));
            sequence.AddChildNode(new SimpleActionNode(action));
            return sequence;
        }
    }
}
