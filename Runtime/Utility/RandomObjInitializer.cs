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
    public class RandomObjInitializer
    {
        [SerializeField] private IDWeightPair[] _prefabIds;

        public GameObject InstantiatePrefab(Vector3 position, Quaternion rotation)
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
                }
            }

            Debug.Assert(id != null);

            return TiyaGameSystem.Pool.InstantiatePrefab(id, position, rotation);
        }

        [System.Serializable]
        public struct IDWeightPair
        {
            [SerializeField] private string _id;
            [SerializeField] private float _weight;

            public string ID => _id;
            public float Weight => _weight;
        }
    }
}
