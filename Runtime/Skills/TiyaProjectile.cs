using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Skill
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
    public class TiyaProjectile : MonoBehaviour
    {
        [TypeRestriction(typeof(IDamageSource))]
        [SerializeField] Object _damageSourceObject;
        [SerializeField] private bool _doDamageToTrigger = true;

        [SerializeField] private bool _destoryWhenDoDamage = true;

        [Header("Debug")]
        [SerializeField] private bool DrawMovementPath = false;

        IDamageSource _damageSource;

        public IWeaponController Weapon { get; set; }

        private void Awake()
        {
            _damageSource = _damageSourceObject.ConvertTo<IDamageSource>();
        }

        private void OnEnable()
        {
            _lastPos = transform.position;
        }

        private Vector3 _lastPos;
        private void Update()
        {
            if (DrawMovementPath)
            {
                // 绘制轨迹
                Debug.DrawLine(transform.position, _lastPos, Color.green, 5f);
                _lastPos = transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_doDamageToTrigger)
            {
                var damagedObject = other.gameObject;
                TryDoDamage(damagedObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var damagedObject = collision.collider.gameObject;
            TryDoDamage(damagedObject);
        }

        private void TryDoDamage(GameObject damagedObject)
        {
            if (_damageSource.DoDamageTo(damagedObject))
            {
                // Destory
                if (_destoryWhenDoDamage)
                {
                    TiyaGameSystem.Pool.RecyclePrefab(gameObject);
                }
            }
        }
    }
}
