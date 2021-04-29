using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 一个随机 Prefab 生成器，会根据权重通过对象池生成随机生成 Prefab。
    /// </summary>
    [System.Serializable]
    public class RandomObjctEmitter
    {
        [SerializeField] private IDWeightPair[] _prefabIds;

        /// <summary>
        /// 生成 Prefab
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <returns>返回生成的 GameObject。如果生成失败则返回 null。</returns>
        public GameObject InstantiatePrefab(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!_prefabIds.Any())
            {
                return null;
            }

            var totalWeight = _prefabIds.Sum(pair => pair.Weight);
            var randomPos = Random.Range(0, totalWeight);

            string id = null;
            foreach (var pair in _prefabIds)
            {
                randomPos -= pair.Weight;
                if (randomPos <= 0)
                {
                    id = pair.ID;
                    break;
                }
            }

            Debug.Assert(id != null);

            var obj = TiyaGameSystem.Pool.InstantiatePrefab(id, position, rotation);

            if (parent)
            {
                obj.transform.SetParent(parent);
            }
            
            return obj;
        }

        [System.Serializable]
        struct IDWeightPair
        {
            [SerializeField] private string _id;
            [SerializeField] private float _weight;

            public string ID => _id;
            public float Weight => _weight;
        }
    }
}
