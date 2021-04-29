namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SelectorNode : CompositeNode
    {
        int _startIndex;

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            for (int i = _startIndex; i < ChildNodes.Count; i++)
            {
                var node = ChildNodes[i];

                switch (node.Update())
                {
                    case BehaviourTreeResult.Success:
                        _startIndex = 0;
                        return BehaviourTreeResult.Success;
                    case BehaviourTreeResult.Failure:
                        continue;
                    case BehaviourTreeResult.Running:
                        _startIndex = i;
                        return BehaviourTreeResult.Running;
                }
            }

            return BehaviourTreeResult.Failure;
        }

        protected internal override void Reset()
        {
            base.Reset();
            _startIndex = 0;
        }
    }
}
