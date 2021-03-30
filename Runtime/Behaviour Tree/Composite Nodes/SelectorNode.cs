namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SelectorNode : CompositeNode
    {
        int _startIndex;

        public SelectorNode(TiyaBehaviourTree behaviourTree) : base(behaviourTree)
        {
        }

        public override BehaviourTreeResult Update()
        {
            for (int i = _startIndex; i < ChildNodes.Count; i++)
            {
                var node = ChildNodes[i];

                switch (node.Update())
                {
                    case BehaviourTreeResult.Success:
                        _startIndex = 0;
                        return NodeResult = BehaviourTreeResult.Success;
                    case BehaviourTreeResult.Failure:
                        continue;
                    case BehaviourTreeResult.Running:
                        _startIndex = i;
                        return NodeResult = BehaviourTreeResult.Running;
                }
            }

            return NodeResult = BehaviourTreeResult.Failure;
        }

        public override void Reset()
        {
            base.Reset();
            _startIndex = 0;
        }
    }
}
