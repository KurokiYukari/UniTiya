using UnityEngine;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// <see cref="WeaponDamageFieldMeleeSkill"/> 的 Behaviour 引用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Weapon Damage Field Melee Skill")]
    public class WeaponDamageFieldMeleeSkillBehaviour : SkillRefBehaviourBase<WeaponDamageFieldMeleeSkill>
    {
        [SerializeField] WeaponDamageFieldMeleeSkill _skill;
        public override WeaponDamageFieldMeleeSkill Skill => _skill;
    }

    /// <summary>
    /// 造成伤害区域的技能
    /// </summary>
    [System.Serializable]
    public class WeaponDamageFieldMeleeSkill : TiyaSkill
    {
        [TiyaPropertyAttributes.TypeRestriction(typeof(IDamageSource))]
        [SerializeField] Object _damageSourceObject;

        [SerializeField] int _colliderBufferCapacity = 16;
        Collider[] _colliderBuffer;
        Collider[] ColliderBuffer => _colliderBuffer ??= new Collider[_colliderBufferCapacity];

        [Header("DamageBox Setting")]
        [SerializeField] Vector3 _halfExtent;
        [SerializeField] Vector3 _centerPositionOffset;
        [SerializeField] Vector3 _rotationOffset;
        [SerializeField] LayerMask _layer = -1;
        [SerializeField] QueryTriggerInteraction _queryTriggerInteraction;

        IDamageSource _damageSource;
        public IDamageSource DamageSource
        {
            get
            {
                if (_damageSourceObject == null)
                {
                    throw new MissingComponentException($"{nameof(WeaponAnimatedMeleeSkill)}'s damageSource can't be null");
                }
                return _damageSource ??= _damageSourceObject.ConvertTo<IDamageSource>();
            }
        }

        IWeaponController _weapon;
        public IWeaponController Weapon => _weapon ??= SkillPerformer.GetComponent<IWeaponController>();

        void PerformAction()
        {
            DamageSource.GenerateBoxDamageField(
                ColliderBuffer,
                _halfExtent,
                Weapon.Owner.ActorTransform.position + _centerPositionOffset,
                Weapon.Owner.ActorTransform.rotation * Quaternion.Euler(_rotationOffset),
                _layer,
                _queryTriggerInteraction);
        }

        protected override void OnInit()
        {
            OnPerforming += PerformAction;
        }
    }
}
