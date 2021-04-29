using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// <see cref="WeaponAnimatedMeleeSkill"/> 的 Behaviour 引用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Weapon Animated Melee Skill")]
    public class WeaponAnimatedMeleeSkillBehaviour : SkillRefBehaviourBase<WeaponAnimatedMeleeSkill>
    {
        [SerializeField] WeaponAnimatedMeleeSkill _skill;
        public override WeaponAnimatedMeleeSkill Skill => _skill;
    }

    /// <summary>
    /// 通过动画进行伤害判定的 Skill。
    /// 要求要进行判定的动画中设置了 <see cref="IMeleeWeaponAnimationEvents"/> 中对应的动画事件。
    /// </summary>
    [System.Serializable]
    public class WeaponAnimatedMeleeSkill : TiyaSkill
    {
        [SerializeField] private List<Transform> _linecastTransforms;
        [SerializeField] uint _checkAttackingPathFrameInterval = 10;
        public uint CheckAttackingPathFrameInterval
        {
            get => _checkAttackingPathFrameInterval;
            set => _checkAttackingPathFrameInterval = value;
        }

        [TypeRestriction(typeof(IDamageSource))]
        [SerializeField] private Object _damageSourceObject;

        [Header("Debug")]
        [SetProperty(nameof(ShowLinecastPath))]
        [SerializeField] private bool _showLinecastPath;
        public bool ShowLinecastPath
        {
            get => _showLinecastPath;
            set => _showLinecastPath = value;
        }

        private WeaponMeleeSkillAnimationEventsHandler _eventsHandler;

        public IEnumerable<Vector3> LinecastPositions =>
            from transform in _linecastTransforms select transform.position;

        IDamageSource _damageSource;
        public IDamageSource DamageSource
        {
            get
            {
                if (_damageSource == null && _damageSourceObject == null)
                {
                    throw new MissingComponentException($"{nameof(WeaponAnimatedMeleeSkill)}'s damageSource can't be null");
                }
                return _damageSource ??= _damageSourceObject.ConvertTo<IDamageSource>();
            }
        }

        private IWeaponController _weapon = null;
        public IWeaponController Weapon => _weapon ??= SkillPerformer.GetComponent<IWeaponController>();
 
        void EnableAction()
        {
            if (Weapon.Owner != null && _eventsHandler == null)
            {
                _eventsHandler = Weapon.Owner.GameObject.AddComponent<WeaponMeleeSkillAnimationEventsHandler>();
                _eventsHandler.Skill = this;
            }
        }
        void DisableAction()
        {
            if (_eventsHandler != null)
            {
                Object.Destroy(_eventsHandler);
                _eventsHandler = null;
            }
        }

        protected override bool OnInit()
        {
            OnEnable += EnableAction;
            OnDisable += DisableAction;

            DamageSource.Producer = Weapon.Owner.GameObject;
            return true;
        }
    }
}
