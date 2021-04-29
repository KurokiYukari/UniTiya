using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.ItemSystem.Listeners
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Listener/Item Generate Sphere Damage Field Listener")]
    public class ItemGenerateSphereDamageFieldListener : ScriptableObject
    {
        [TypeRestriction(typeof(IDamageSource))]
        [SerializeField] Object _damageSourceObject;

        [SerializeField] int _colliderBufferCapacity = 16;
        Collider[] _colliderBuffer;

        [Header("DamageSphere Setting")]
        [SerializeField] float _radius;
        [SerializeField] Vector3 _centerPositionOffset;
        [SerializeField] LayerMask _layer = -1;
        [SerializeField] QueryTriggerInteraction _queryTriggerInteraction;

        IDamageSource _damageSource;
        public IDamageSource DamageSource
        {
            set => _damageSource = value;
            get => _damageSource = _damageSource ?? _damageSourceObject.ConvertTo<IDamageSource>();
        }

        private void Awake()
        {
            _colliderBuffer = new Collider[_colliderBufferCapacity];
        }

        public void GenerateDamageField(IItem item, IActorController actor) =>
            DamageSource.GenerateSphereDamageField(
                _colliderBuffer,
                _radius,
                actor.ActorTransform.position + _centerPositionOffset,
                _layer,
                _queryTriggerInteraction);
    }
}
