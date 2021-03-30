using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarachan.UniTiya.BehaviourTree
{
    public abstract class BehaviourTreeNodeBase
    {
        public TiyaBehaviourTree BehaviourTree { get; }

        public BehaviourTreeNodeType NodeType { get; }
        public BehaviourTreeResult NodeResult { get; protected set; } = 0;

        internal BehaviourTreeNodeBase(BehaviourTreeNodeType nodeType, TiyaBehaviourTree behaviourTree)
        {
            NodeType = nodeType;
            BehaviourTree = behaviourTree;
        }

        public virtual void Reset()
        {
            NodeResult = 0;
        }

        public abstract BehaviourTreeResult Update();

        public bool CanRun => !(NodeResult == BehaviourTreeResult.Running);
    }

    public enum BehaviourTreeNodeType
    {
        Composite,
        Decorator,
        Leaf
    }

    public enum BehaviourTreeResult
    {
        Success, Failure, Running
    }

    public abstract class CompositeNode : BehaviourTreeNodeBase
    {
        public CompositeNode(TiyaBehaviourTree behaviourTree) : base(BehaviourTreeNodeType.Composite, behaviourTree) { }

        public List<BehaviourTreeNodeBase> ChildNodes { get; } = new List<BehaviourTreeNodeBase>();
    }

    public abstract class DecoratorNode : BehaviourTreeNodeBase
    {
        public DecoratorNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(BehaviourTreeNodeType.Decorator, behaviourTree)
        {
            DecoratedNode = decoratedNode;
        }

        protected BehaviourTreeNodeBase DecoratedNode { get; set; }
    }

    public abstract class LeafNode : BehaviourTreeNodeBase
    {
        public LeafNode(TiyaBehaviourTree behaviourTree) : base(BehaviourTreeNodeType.Leaf, behaviourTree) { }
    }
}
