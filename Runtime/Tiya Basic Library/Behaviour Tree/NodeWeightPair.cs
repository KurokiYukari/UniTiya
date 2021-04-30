using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 序列化的 <see cref="BehaviourTreeNodeBase"/> 与一个权重的组合。
    /// 该结构体一般用于配置拥有随机元素的 <see cref="CompositeNode"/> 的子节点，
    /// 比如 <see cref="RandomSelectorNode"/> 的子节点之类的。
    /// </summary>
    [System.Serializable]
    public struct NodeWeightPair
    {
        [TypeRestriction(typeof(IBehaviourTreeNodeReference))]
        [SerializeField] Object _nodeReferenceObject;
        [SerializeField] float _weight;

        BehaviourTreeNodeBase _node;
        public BehaviourTreeNodeBase Node
        {
            get
            {
                if (!_nodeReferenceObject)
                {
                    throw new MissingReferenceException(nameof(_nodeReferenceObject));
                }

                if (_node == null)
                {
                    var nodeObjCopy = Object.Instantiate(_nodeReferenceObject);
                    _node = nodeObjCopy.ConvertTo<IBehaviourTreeNodeReference>().Node;
                }

                return _node;
            }
        }

        public float Weight => _weight;
    }
}
