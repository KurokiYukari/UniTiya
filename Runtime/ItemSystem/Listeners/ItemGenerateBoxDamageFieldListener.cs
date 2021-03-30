using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.ItemSystem.Listeners
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Listener/Item Generate Box Damage Field Listener")]
    public class ItemGenerateBoxDamageFieldListener : ScriptableObject
    {
        [TypeRestriction(typeof(IDamageSource))]
        [SerializeField] Object _damageSourceObject;

        [SerializeField] int _colliderBufferCapacity = 16;
        Collider[] _colliderBuffer;

        [Header("DamageBox Setting")]
        [SerializeField] Vector3 _halfExtent;
        [SerializeField] Vector3 _centerPositionOffset;
        [SerializeField] Vector3 _rotationOffset;
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
            DamageSource.GenerateBoxDamageField(
                _colliderBuffer,
                _halfExtent,
                actor.ActorTransform.position + _centerPositionOffset,
                actor.ActorTransform.rotation * Quaternion.Euler(_rotationOffset),
                _layer,
                _queryTriggerInteraction);
    }
}
