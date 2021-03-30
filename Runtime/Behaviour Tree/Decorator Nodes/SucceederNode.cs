namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class SucceederNode : DecoratorNode
    {
        public SucceederNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(behaviourTree, decoratedNode)
        {
        }

        public override BehaviourTreeResult Update()
        {
            return NodeResult = DecoratedNode.Update() switch
            {
                BehaviourTreeResult.Running => BehaviourTreeResult.Running,
                _ => BehaviourTreeResult.Success
            };
        }
    }
}
