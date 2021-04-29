using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 所有行为树节点的基类。
    /// </summary>
    public abstract class BehaviourTreeNodeBase
    {
        TiyaBehaviourTree _behaviourTree;
        public TiyaBehaviourTree BehaviourTree
        {
            get => _behaviourTree ? _behaviourTree :
                throw new System.InvalidOperationException($"{nameof(BehaviourTree)} is null. You must call {nameof(Init)} method before use the node");
            private set => _behaviourTree = value;
        }

        /// <summary>
        /// 节点的返回值
        /// </summary>
        public BehaviourTreeResult NodeResult { get; private set; } = 0;

        public event System.Action OnUpdate;
        public event System.Action OnReset;

        /// <summary>
        /// 节点的初始化方法。该方法会在其所属的 <see cref="BehaviourTree"/> 的 Start 时调用。
        /// 该方法一般用来 Init 可能会有的一些不在通过 <see cref="NodesEnumerable"/> 得到的树结构的一些中间节点或
        /// 初始化各种与 <see cref="BehaviourTree"/> 相关联的成员，比如寻找组件等行为。
        /// </summary>
        /// <param name="behaviourTree">所属的行为树</param>
        internal protected virtual void Init(TiyaBehaviourTree behaviourTree)
        {
            BehaviourTree = behaviourTree ? behaviourTree : throw new System.ArgumentNullException(nameof(behaviourTree));
        }

        /// <summary>
        /// 将节点重置为初始状态。
        /// </summary>
        internal protected virtual void Reset()
        {
            NodeResult = 0;

            OnReset?.Invoke();
        }

        /// <summary>
        /// 执行该节点
        /// </summary>
        /// <returns></returns>
        public BehaviourTreeResult Update()
        {
            if (CanRun)
            {
                Reset();
            }

            OnUpdate?.Invoke();
            return NodeResult = OnUpdateOverride();
        }

        /// <summary>
        /// 通过重写这个方法来实现节点的行为。
        /// </summary>
        /// <returns>节点执行的结果</returns>
        protected abstract BehaviourTreeResult OnUpdateOverride();

        /// <summary>
        /// 节点是否可以执行（处于初始状态或已经执行完毕）
        /// </summary>
        public bool CanRun => !(NodeResult == BehaviourTreeResult.Running);

        /// <summary>
        /// 按照行为树执行顺序获取包括此节点的所有子节点。
        /// </summary>
        public abstract IEnumerable<BehaviourTreeNodeBase> NodesEnumerable { get; }

        #region ---- Component Methods ----
        public GameObject GameObject => BehaviourTree.gameObject;
        public Transform Transform => BehaviourTree.transform;

#pragma warning disable UNT0014 // Invalid type for call to GetComponent
        public T GetComponent<T>() => BehaviourTree.GetComponent<T>();
#pragma warning restore UNT0014 // Invalid type for call to GetComponent
        #endregion
    }

    /// <summary>
    /// 对 <see cref="BehaviourTreeNodeBase"/> 的引用。
    /// </summary>
    public interface IBehaviourTreeNodeReference
    {
        BehaviourTreeNodeBase Node { get; }
    }

    public enum BehaviourTreeResult
    {
        Success, Failure, Running
    }

    /// <summary>
    /// Composite 类型节点的基类。
    /// </summary>
    public abstract class CompositeNode : BehaviourTreeNodeBase
    {
        readonly List<BehaviourTreeNodeBase> _childNodes = new List<BehaviourTreeNodeBase>();

        public IReadOnlyList<BehaviourTreeNodeBase> ChildNodes => _childNodes;

        public sealed override IEnumerable<BehaviourTreeNodeBase> NodesEnumerable
        {
            get
            {
                yield return this;

                foreach (var childNode in ChildNodes)
                {
                    foreach (var node in childNode.NodesEnumerable)
                    {
                        yield return node;
                    }
                }
            }
        }

        public virtual void AddChildNode(BehaviourTreeNodeBase node) => _childNodes.Add(node);
    }

    /// <summary>
    /// Decorator 类型节点的基类。
    /// </summary>
    public abstract class DecoratorNode : BehaviourTreeNodeBase
    {
        public DecoratorNode(BehaviourTreeNodeBase decoratedNode)
        {
            DecoratedNode = decoratedNode;
        }

        protected BehaviourTreeNodeBase DecoratedNode { get; }

        public sealed override IEnumerable<BehaviourTreeNodeBase> NodesEnumerable
        {
            get
            {
                yield return this;

                foreach (var node in DecoratedNode.NodesEnumerable)
                {
                    yield return node;
                }
            }
        }
    }

    /// <summary>
    /// Leaf（Action） 类型节点的基类
    /// </summary>
    public abstract class LeafNode : BehaviourTreeNodeBase
    {
        public sealed override IEnumerable<BehaviourTreeNodeBase> NodesEnumerable
        {
            get
            {
                yield return this;
            }
        }
    }
}
