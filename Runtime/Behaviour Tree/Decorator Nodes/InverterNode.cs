namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class InverterNode : DecoratorNode
    {
        public InverterNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(behaviourTree, decoratedNode)
        {
        }

        public override BehaviourTreeResult Update()
        {
            return NodeResult = DecoratedNode.Update() switch
            {
                BehaviourTreeResult.Success => BehaviourTreeResult.Failure,
                BehaviourTreeResult.Failure => BehaviourTreeResult.Success,
                BehaviourTreeResult.Running => BehaviourTreeResult.Running,
                _ => throw new System.InvalidOperationException(),
            };
        }
    }
}
