using UnityEngine;
using UnityEngine.Events;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 基础的可配置 DamageableSource
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Damage Interaction/Tiya Basic DamageSource")]
    public class TiyaBasicDamageSource : MonoBehaviour, IDamageSource
    {
        [SerializeField] GameObject _overrideProducer;

        [SerializeField] float _damageValue;

        [SerializeField] DamageEvent _onBeforeDoDamage;
        [SerializeField] DamageEvent _onAfterDoDamage;

        public event System.Action<IDamageSource, IDamageable> OnBeforeDoDamageEvent;
        public event System.Action<IDamageSource, IDamageable> OnAfterDoDamageEvent;

        public virtual GameObject Producer
        {
            get
            {
                if (_overrideProducer == null)
                {
                    _overrideProducer = gameObject;
                }
                return _overrideProducer;
            }
        }

        public virtual float OriginDamageValue { get => _damageValue; set => _damageValue = value; }
        public float FinalDamageValue { get; protected set; }

        public void OnBeforeDoDamage(IDamageable damageable)
        {
            FinalDamageValue = OriginDamageValue;

            _onBeforeDoDamage.Invoke(this, damageable);
            OnBeforeDoDamageEvent?.Invoke(this, damageable);

            OnBeforeDoDamageOverride(damageable);
        }
        protected virtual void OnBeforeDoDamageOverride(IDamageable damageable) { }

        public void OnAfterDoDamage(IDamageable damageable)
        {
            _onAfterDoDamage.Invoke(this, damageable);
            OnAfterDoDamageEvent?.Invoke(this, damageable);

            OnAfterDoDamageOverride(damageable);
        }
        protected virtual void OnAfterDoDamageOverride(IDamageable damageable) { }

        [System.Serializable]
        public class DamageEvent : UnityEvent<IDamageSource, IDamageable> { }
    }
}
