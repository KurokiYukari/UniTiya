using System.Collections.Generic;

namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class RandomSelectorNode : CompositeNode
    {
        int _startIndex;
        readonly List<BehaviourTreeNodeBase> _shuffledNodes = new List<BehaviourTreeNodeBase>();

        public RandomSelectorNode(TiyaBehaviourTree behaviourTree) : base(behaviourTree)
        {
        }

        public override BehaviourTreeResult Update()
        {
            if (!(NodeResult == BehaviourTreeResult.Running))
            {
                _startIndex = 0;
                _shuffledNodes.Clear();
                ChildNodes.Shuffle(_shuffledNodes);
            }

            for (int i = _startIndex; i < _shuffledNodes.Count; i++)
            {
                var node = _shuffledNodes[i];

                switch (node.Update())
                {
                    case BehaviourTreeResult.Success:
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
            _shuffledNodes.Clear();
        }
    }
}
