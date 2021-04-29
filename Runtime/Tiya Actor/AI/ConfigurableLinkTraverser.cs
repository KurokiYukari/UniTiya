namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// 一个运行时的 <see cref="IActorNavMeshLinkTraverser"/> 实现，通过事件配置跨越 Link 的方法。
    /// </summary>
    public class ConfigurableLinkTraverser : IActorNavMeshLinkTraverser
    {
        public event System.Action<TiyaActorMoveAI> OnTraverseLink;

        public ConfigurableLinkTraverser(System.Action<TiyaActorMoveAI> linkTraverseAction)
        {
            OnTraverseLink += linkTraverseAction ?? throw new System.ArgumentNullException();
        }

        public void TraverseNavMeshLink(TiyaActorMoveAI actorMoveAI) => OnTraverseLink.Invoke(actorMoveAI);
    }
}
