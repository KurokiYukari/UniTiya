using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sarachan.UniTiya.DamageInteract
{
    /// <summary>
    /// 一个可配置的 IDamageable 实现，它可以通过配置事件、或继承重写 <see cref="ReceiveDamageOverride(IDamageSource)"/>
    /// 方法来配置 Damageable 行为。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Damage Interaction/Tiya Damageable")]
    public class TiyaDamageable : MonoBehaviour, IDamageable
    {
        [SerializeField] GameObject _overrideReceiver;

        [SerializeField] DamageEvent _onReceiveDamage;

        public GameObject Receiver => _overrideReceiver ??= gameObject;

        public bool IsInvulnerable { get => !enabled; set => enabled = !value; }

        public event System.Action<IDamageable, IDamageSource> OnReceiveDamageEvent;

        public void ReceiveDamage(IDamageSource damageSource) 
        {
            _onReceiveDamage.Invoke(this, damageSource);
            OnReceiveDamageEvent?.Invoke(this, damageSource);

            ReceiveDamageOverride(damageSource);
        }
        protected virtual void ReceiveDamageOverride(IDamageSource damageSource) { }

        [System.Serializable]
        class DamageEvent : UnityEvent<IDamageable, IDamageSource> { }
    }
}
