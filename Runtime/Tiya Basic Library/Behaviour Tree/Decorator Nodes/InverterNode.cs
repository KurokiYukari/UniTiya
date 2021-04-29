namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class InverterNode : DecoratorNode
    {
        public InverterNode(BehaviourTreeNodeBase decoratedNode) : base(decoratedNode)
        {
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            return DecoratedNode.Update() switch
            {
                BehaviourTreeResult.Success => BehaviourTreeResult.Failure,
                BehaviourTreeResult.Failure => BehaviourTreeResult.Success,
                BehaviourTreeResult.Running => BehaviourTreeResult.Running,
                _ => throw new System.InvalidOperationException(),
            };
        }
    }
}
