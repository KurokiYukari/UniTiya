using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Sarachan.UniTiya
{
    public static partial class TiyaTools
    {
#if UNITY_EDITOR
        public const string UniTiyaName = "UniTiya";
#endif

        public static class TiyaLayers
        {
            public const string Player = "Player";
            public const string PlayerWeapon = "Player Weapon";
        }

        public static void SetChildrenLayerTo(this GameObject obj, string layerName, System.Predicate<GameObject> predicate = null)
        {
            SetChildrenLayerTo(obj, LayerMask.NameToLayer(layerName), predicate);
            //foreach (var trans in obj.GetComponentsInChildren<Transform>())
            //{
            //    if (predicate == null || predicate(trans.gameObject))
            //    {
            //        trans.gameObject.layer = LayerMask.NameToLayer(layerName);
            //    }
            //}
        }

        public static void SetChildrenLayerTo(this GameObject obj, int layer, System.Predicate<GameObject> predicate = null)
        {
            foreach (var trans in obj.GetComponentsInChildren<Transform>())
            {
                if (predicate == null || predicate(trans.gameObject))
                {
                    trans.gameObject.layer = layer;
                }
            }
        }

        /// <summary>
        /// 从 unityObject 寻找 T 类型的对象。
        /// 如果 unityObject 是 GameObject 或 Component，则从其组件中寻找。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityObject"></param>
        /// <returns>如果没有找到，返回 null</returns>
        public static T ConvertTo<T>(this Object unityObject) where T : class
        {
            if (unityObject is T result)
            {
                return result;
            }

            var gameObject = unityObject as GameObject;
            if (gameObject != null)
            {
                return gameObject.GetComponent(typeof(T)) as T;
            }

            var component = unityObject as Component;
            if (component != null)
            {
                return component.GetComponent(typeof(T)) as T;
            }

            return null;
        }

        public static void Shuffle<T>(this IEnumerable<T> originEnumerable, IList<T> targetList)
        {
            targetList.Clear();
            var random = new System.Random();
            foreach (var item in originEnumerable)
            {
                targetList.Insert(random.Next(targetList.Count + 1), item);
            }
        }

        /// <summary>
        /// 拥有权重的将 <paramref name="originEnumerable"/> 随机重新排序到 <paramref name="targetList"/>。
        /// 拥有较大权重的元素会更倾向于排序到 <paramref name="targetList"/> 的前面。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originEnumerable"></param>
        /// <param name="weights">元素的权重，它按照顺序与 <paramref name="originEnumerable"/> 元素一一匹配</param>
        /// <param name="targetList"></param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="originEnumerable"/> 与 <paramref name="weights"/> 长度不一致时引发的异常。
        /// </exception>
        public static void Shuffle<T>(this IEnumerable<T> originEnumerable, IEnumerable<float> weights, IList<T> targetList)
        {
            targetList.Clear();
            var originList = originEnumerable.ToList();
            var weightList = weights.ToList();
            var count = originList.Count;
            if (count != weightList.Count)
            {
                throw new System.InvalidOperationException($"Param {originEnumerable}'s count must be the same as {weights}'s count.");
            }
            var totalWeight = weightList.Sum();

            for (int i = 0; i < count; i++)
            {
                var position = Random.value * totalWeight;
                for (int j = 0; j < weightList.Count; j++)
                {
                    if (position <= weightList[j])
                    {
                        totalWeight -= weightList[j];
                        targetList.Add(originList[j]);
                        originList.RemoveAt(j);
                        weightList.RemoveAt(j);
                        break;
                    }
                    else
                    {
                        position -= weightList[j];
                    }
                }
            }
        }

        /// <summary>
        /// 向 EventTrigger 添加 Listener
        /// </summary>
        /// <param name="eventTrigger"></param>
        /// <param name="triggerType"></param>
        /// <param name="action"></param>
        public static void AddEventTriggerListener(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            EventTrigger.Entry entry = eventTrigger.triggers.Find((trigger) => trigger.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry() { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(action);
        }

        public static void GenerateBoxDamageField(
            this IDamageSource damageSource,
            Collider[] colliersBuffer,
            Vector3 halfExtent,
            Vector3 centerPosition, 
            Quaternion? rotation = null,
            int mask = -1,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var collidersCount = Physics.OverlapBoxNonAlloc(centerPosition, halfExtent, colliersBuffer, rotation ?? Quaternion.identity, mask, queryTriggerInteraction);
            for (int i = 0; i < collidersCount; i++)
            {
                damageSource.DoDamageTo(colliersBuffer[i].gameObject);
            }
        }
        public static void GenerateSphereDamageField(
            this IDamageSource damageSource,
            Collider[] collidersBuffer,
            float radius,
            Vector3 centerPosition,
            int mask = -1,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var collidersCount = Physics.OverlapSphereNonAlloc(centerPosition, radius, collidersBuffer, mask, queryTriggerInteraction);
            for (int i = 0; i < collidersCount; i++)
            {
                damageSource.DoDamageTo(collidersBuffer[i].gameObject);
            }
        }
    }
}
