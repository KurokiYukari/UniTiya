using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

using Sarachan.UniTiya.TiyaPropertyAttributes;
using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// 投掷物组件。注意，该组件默认是 Disable 的，需要手动 enable。
    /// Enable 状态的该组件才会造成伤害。
    /// </summary>
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(TrailRenderer))]
    public class TiyaProjectile : MonoBehaviour
    {
        [TypeRestriction(typeof(IDamageSource))]
        [SerializeField] Object _damageSourceObject;

        // FIXME: 高速状态下，对 collider 能触发而对 trigger 不能？
        //[SerializeField] private bool _doDamageToTrigger = true;

        [SerializeField] private bool _destoryWhenDoDamage = true;

        public IDamageSource DamageSource { get; private set; }

        public IWeaponController Weapon { get; set; }

        public TrailRenderer TrailRenderer { get; private set; }

        public IEnable DamageEnabler { get; private set; }

        public event System.Action<Collision> OnProjectileCollision;

        System.IDisposable _enableTrailSubscribe;
        private void Awake()
        {
            DamageSource = _damageSourceObject.ConvertTo<IDamageSource>();
            TrailRenderer = GetComponent<TrailRenderer>();

            DamageEnabler = new TiyaEnableAgent(OnDamageEnableListener, OnDamageDisableListener);

            void OnDamageEnableListener()
            {
                DamageSource.Producer = Weapon.Owner.GameObject;

               // 延时一帧，防止将 enable 前的位置也计算到轨迹绘制中
               _enableTrailSubscribe = Observable.NextFrame().Subscribe(_ =>
               {
                   TrailRenderer.emitting = true;
               });
            }
            void OnDamageDisableListener()
            {
                _enableTrailSubscribe?.Dispose();
                TrailRenderer.emitting = false;
            }
        }

        private void Start()
        {
            TrailRenderer.emitting = false;
        }

        //private void OnEnable()
        //{
        //    DamageEnabler.Enable();
        //}

        private void OnDisable()
        {
            DamageEnabler.Disable();
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    print("Trigger Enter");
        //    if (_doDamageToTrigger && DamageEnabler.Enabled)
        //    {
        //        var damagedObject = other.gameObject;
        //        TryDoDamage(damagedObject);
        //    }
        //}

        //private void OnTriggerStay(Collider other)
        //{
        //    print("Trigger Stay");
        //    if (_doDamageToTrigger && DamageEnabler.Enabled)
        //    {
        //        var damagedObject = other.gameObject;
        //        TryDoDamage(damagedObject);
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    print("Trigger Exit");
        //    if (_doDamageToTrigger && DamageEnabler.Enabled)
        //    {
        //        var damagedObject = other.gameObject;
        //        TryDoDamage(damagedObject);
        //    }
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if (DamageEnabler.Enabled)
            {
                OnProjectileCollision?.Invoke(collision);

                var damagedObject = collision.collider.gameObject;

                TryDoDamage(damagedObject, true);
            }
        }

        private bool TryDoDamage(GameObject damagedObject, bool disableIfDoDamageFailed = false)
        {
            var result = DamageSource.DoDamageTo(damagedObject);
            if (result)
            {
                // Destory
                if (_destoryWhenDoDamage)
                {
                    TiyaGameSystem.Pool.RecyclePrefab(gameObject);
                }
            }
            else
            {
                if (disableIfDoDamageFailed)
                {
                    DamageEnabler.Disable();
                }
            }

            return result;
        }
    }
}
