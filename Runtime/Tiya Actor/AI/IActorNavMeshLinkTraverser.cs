namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// Actor 寻路时遇到 Manual Link 时该如何跨越 Link 的接口。应该将实现该接口的组件添加
    /// 到 Link 组件所在的 GameObject 上。
    /// <para>另见 <seealso cref="TiyaActorMoveAI"/>。</para>
    /// </summary>
    public interface IActorNavMeshLinkTraverser
    {
        /// <summary>
        /// actorMoveAI.Actor 跨越 actorMoveAI.Agent 当前所在 Link 的逻辑
        /// </summary>
        /// <param name="actorMoveAI"></param>
        void TraverseNavMeshLink(TiyaActorMoveAI actorMoveAI);
    }
}
