using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    public interface IGameObjectPool
    {
        /// <summary>
        /// 根据 prefabId 获取对应的 GameObject
        /// </summary>
        /// <param name="prefabId"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        GameObject InstantiatePrefab(string prefabId, Vector3 position, Quaternion rotation);

        /// <summary>
        /// 回收 gameObject。如果 gameObject 不存在于池中，则直接 Destory
        /// </summary>
        /// <param name="gameObject"></param>
        void RecyclePrefab(GameObject gameObject);
    }
}
