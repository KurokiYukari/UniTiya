using UnityEngine;
using UnityEngine.Events;

namespace Sarachan.UniTiya
{
    // TODO：用 IGameProperties 拓展 DamageSource
    /// <summary>
    /// 伤害源
    /// </summary>
    public interface IDamageSource
    {
        /// <summary>
        /// 伤害制造者。
        /// </summary>
        GameObject Producer { get; set; }

        /// <summary>
        /// 基础伤害数值
        /// </summary>
        float OriginDamageValue { get; set; }
        /// <summary>
        /// 最终伤害数值
        /// </summary>
        float FinalDamageValue { get; }

        // --------
        // 下面两个回调一般用来自定义 DamageSource 的特殊逻辑。
        // 如果是使用 IDamageSource.DoDamageTo 进行 Damage 判定，则这两个回调已经被处理。

        /// <summary>
        /// Damageable RecieveDamage 前回调
        /// </summary>
        /// <param name="damageable"></param>
        void OnBeforeDoDamage(IDamageable damageable);

        /// <summary>
        /// Damageable RecieveDamage 后回调
        /// </summary>
        /// <param name="damageable"></param>
        void OnAfterDoDamage(IDamageable damageable);

        event System.Action<IDamageSource, IDamageable> OnBeforeDoDamageEvent;
        event System.Action<IDamageSource, IDamageable> OnAfterDoDamageEvent;

        //
        // --------
    }

    public static partial class TiyaTools
    {
        /// <summary>
        /// 尝试用 damageSource 触发一个 GameObject 上的第一个 IDamageable。
        /// 这个方法已经处理了 damageSource OnDoDamage 事件。
        /// 触发要求：targetObject 中有 IDamageable 组件，
        ///     且 damageSource 的 Producer 不等于 targetObject，
        ///     且 shouldDoDamageFunc 返回 true，
        ///     且 damageable.IsInvulnerable 为 false
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="targetObject"></param>
        /// <param name="shouldDoDamageFunc">默认返回 true</param>
        /// <returns>触发成功返回 true，没有触发返回 false</returns>
        public static bool DoDamageTo(this IDamageSource damageSource, GameObject targetObject, System.Func<bool> shouldDoDamageFunc = null)
        {
            var damageable = targetObject.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsInvulnerable
                && damageSource.Producer != damageable.Receiver
                && (shouldDoDamageFunc == null || shouldDoDamageFunc()))
            {
                damageSource.OnBeforeDoDamage(damageable);
                damageable.ReceiveDamage(damageSource);
                damageSource.OnAfterDoDamage(damageable);

                return true;
            }

            //if (damageSource.Producer != targetObject
            //    && (shouldDoDamageFunc == null || shouldDoDamageFunc()))
            //{
                
            //    if (damageable != null && !damageable.IsInvulnerable)
            //    {
            //        damageSource.OnBeforeDoDamage(damageable);
            //        damageable.ReceiveDamage(damageSource);
            //        damageSource.OnAfterDoDamage(damageable);

            //        return true;
            //    }
            //}

            return false;
        }
    }
}
