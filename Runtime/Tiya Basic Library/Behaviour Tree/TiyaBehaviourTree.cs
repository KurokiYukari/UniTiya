using UnityEngine;

using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 行为树实现。通过继承该类，在 Awake 中配置行为树的所有节点。
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
        abstract public BehaviourTreeNodeBase RootNode { get; }

        protected void Start()
        {
            foreach (var node in RootNode.NodesEnumerable)
            {
                node.Init(this);
                node.Reset();
            }
        }

        protected void Update()
        {
            RootNode.Update();
        }
    }
}
