using UnityEngine;
using Sarachan.UniTiya.Utility;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(TiyaProjectile))]
    public class Projectile_CollisionEffectExtension : MonoBehaviour
    {
        public TiyaProjectile Projectile { get; private set; }

        [Header("PhysicMaterial - EffectPrefabId Pair Setting")]
        [SerializeField] RandomObjctEmitter[] _defaultEffectEmmiters;
        [SerializeField] bool _nullMaterialSameAsDefault = true;
        [HideIf(nameof(_nullMaterialSameAsDefault), true)]
        [SerializeField] RandomObjctEmitter[] _nullMaterialEffectEmitters;
        [SerializeField] PhysicMaterial_RandomObjEmitters_Dictionary _effectIdsDictionary;

        protected void Awake()
        {
            Projectile = GetComponent<TiyaProjectile>();
        }

        protected void OnEnable()
        {
            Projectile.OnProjectileCollision += ProjectileCollisionListener;
        }

        protected void OnDisable()
        {
            Projectile.OnProjectileCollision -= ProjectileCollisionListener;
        }

        void ProjectileCollisionListener(Collision collision)
        {
            var material = collision.collider.sharedMaterial;
            var contact = collision.GetContact(0);

            if (material == null)
            {
                if (_nullMaterialSameAsDefault)
                {
                    foreach (var emitter in _defaultEffectEmmiters)
                    {
                        emitter.InstantiatePrefab(contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal), collision.transform);
                    }
                }
                else
                {
                    foreach (var emitter in _nullMaterialEffectEmitters)
                    {
                        emitter.InstantiatePrefab(contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal), collision.transform);
                    }
                }
            }
            else if (_effectIdsDictionary.TryGetValue(material, out var storage))
            {
                foreach (var emitter in storage)
                {
                    emitter.InstantiatePrefab(contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal), collision.transform);
                }
            }
            else
            {
                foreach (var emitter in _defaultEffectEmmiters)
                {
                    emitter.InstantiatePrefab(contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal), collision.transform);
                }
            }
        }

        #region Inner Classes
        [System.Serializable]
        public class PhysicMaterial_RandomObjEmitters_Dictionary 
            : SerializableDictionary<PhysicMaterial, RandomObjctEmitter[], PhysicMaterial_RandomObjEmitters_Dictionary.Storage> 
        {
            [System.Serializable]
            public class Storage : SerializableDictionary.Storage<RandomObjctEmitter[]> { }
        }
        #endregion
    }
}
