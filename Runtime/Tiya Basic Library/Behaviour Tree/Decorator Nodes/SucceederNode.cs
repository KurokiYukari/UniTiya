namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SucceederNode : DecoratorNode
    {
        public SucceederNode(BehaviourTreeNodeBase decoratedNode) : base(decoratedNode)
        {
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return DecoratedNode.Update() switch
            {
                BehaviourTreeResult.Running => BehaviourTreeResult.Running,
                _ => BehaviourTreeResult.Success
            };
        }
    }
}
