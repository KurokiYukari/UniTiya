using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class RandomSequenceNode : CompositeNode
    {
        readonly List<float> _childNodesWeights = new List<float>();

        int _startIndex;
        readonly List<BehaviourTreeNodeBase> _shuffledNodes = new List<BehaviourTreeNodeBase>();

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            for (int i = _startIndex; i < _shuffledNodes.Count; i++)
            {
                var node = _shuffledNodes[i];

                switch (node.Update())
                {
                    case BehaviourTreeResult.Success:
                        continue;
                    case BehaviourTreeResult.Failure:
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
            _shuffledNodes.Clear();
            ChildNodes.Shuffle(_childNodesWeights, _shuffledNodes);
        }

        /// <summary>
        /// 添加权重为 1 的子节点
        /// </summary>
        /// <param name="node"></param>
        public override void AddChildNode(BehaviourTreeNodeBase node)
        {
            AddChildNode(node, 1);
        }

        public void AddChildNode(BehaviourTreeNodeBase node, float weight)
        {
            if (weight < 0)
            {
                Debug.LogWarning($"Node's weight can't be a negative value. Weight will be set to 1.0f.");
                weight = 1f;
            }

            base.AddChildNode(node);
            _childNodesWeights.Add(weight);
        }
    }
}
