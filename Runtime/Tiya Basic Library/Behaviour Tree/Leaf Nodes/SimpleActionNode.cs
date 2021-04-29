using System;

namespace Sarachan.UniTiya.BehaviourTree
{
    public class SimpleActionNode : LeafNode
    {
        BehaviourTreeResult Result { get; }

        public SimpleActionNode(Action nodeAction, Action resetAction = null, BehaviourTreeResult result = BehaviourTreeResult.Success) 
        {
            if (nodeAction != null)
            {
                OnUpdate += nodeAction;
            }

            if (resetAction != null)
            {
                OnReset += resetAction;
            }

            Result = result;
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return Result;
        }

        static readonly SimpleActionNode _nullNode = new SimpleActionNode(null);
        public static SimpleActionNode NullNode => _nullNode;
    }
}
