using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 受到伤害接口
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 伤害的接受者
        /// </summary>
        GameObject Receiver { get; }

        IEnable DamageableEnabler { get; }

        /// <summary>
        /// 是否接受 DamageSource 的伤害
        /// </summary>
        bool IsInvulnerable { get; set; }

        /// <summary>
        /// 接受伤害。
        /// </summary>
        /// <param name="damageSource"></param>
        void ReceiveDamage(IDamageSource damageSource);

        event System.Action<IDamageable, IDamageSource> OnReceiveDamageEvent;
    }
}
