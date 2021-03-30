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

        /// <summary>
        /// 从 unityObject 寻找 T 类型的对象。
        /// 如果 unityObject 是 GameObject，则从其组件中寻找。
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

            return null;
        }

        public static void Shuffle<T>(this IList<T> originList, IList<T> targetList)
        {
            targetList.Clear();
            var random = new System.Random();
            foreach (var item in originList)
            {
                targetList.Insert(random.Next(targetList.Count), item);
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

    //public interface IObject
    //{
    //    Object Object { get; }
    //}

    //public interface IGameObject : IObject
    //{
    //    new GameObject Object { get; }
    //}

    /// <summary>
    /// 有 Enable 功能的接口。
    /// 不建议给 Monobehaviour 之类的本来就有 Enable 功能的类实现该接口，
    /// 而是只给自定义类使用。
    /// </summary>
    public interface IEnable
    {
        bool Enabled { get; set; }
        void Enable();
        void Disable();

        event System.Action OnEnable;
        event System.Action OnDisable;
    }
}
