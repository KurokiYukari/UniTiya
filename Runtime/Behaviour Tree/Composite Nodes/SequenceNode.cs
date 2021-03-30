namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SequenceNode : CompositeNode
    {
        int _startIndex = 0;

        public SequenceNode(TiyaBehaviourTree behaviourTree) : base(behaviourTree)
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
                        continue;
                    case BehaviourTreeResult.Failure:
                        _startIndex = 0;
                        return NodeResult = BehaviourTreeResult.Failure;
                    case BehaviourTreeResult.Running:
                        _startIndex = i;
                        return NodeResult = BehaviourTreeResult.Running;
                }
            }

            return NodeResult = BehaviourTreeResult.Success;
        }

        public override void Reset()
        {
            base.Reset();

            _startIndex = 0;
        }
    }
}
