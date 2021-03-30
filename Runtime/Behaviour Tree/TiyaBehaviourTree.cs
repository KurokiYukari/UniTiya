using UnityEngine;

using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 行为树实现
    /// </summary>
    public abstract class TiyaBehaviourTree : MonoBehaviour
    {
        /// <summary>
        /// 行为树的公用数据空间
        /// </summary>
        public IGameProperties GlobalData { get; } = new RuntimePropertyConfiguration();

        /// <summary>
        /// 行为树的根节点
        /// </summary>
        abstract protected BehaviourTreeNodeBase RootNode { get; }

        protected void Update()
        {
            RootNode.Update();
        }
    }
}
