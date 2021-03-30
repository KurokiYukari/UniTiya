using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using Sarachan.UniTiya.Skill;
using Sarachan.UniTiya.TiyaAnimator;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaWeapon
{
    /// <summary>
    /// 可配置的武器，通过组合 Skill 构造武器。
    /// ExtraSkills 可以定义对应的 InputAction；InputAction 只有在 Owner.IsPlayer 的情况下才会生效。
    /// 所有该武器的 Skill 和 InputAction 的 Enable 只会在改武器 Owner 已被设置的情况下发生，会在设置 Owner 时、组件 Enable 时调用；
    /// 而所有 Skil 和 InputAction 的 Disable 会在组件 Disable 时调用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Tiya Weapon Controller")]
    public class TiyaWeaponController : MonoBehaviour, IWeaponController
    {
        [Tooltip("Unity Object or GameObject contains Component implements interface IWeaponActorActions.")]
        [SerializeField] [TypeRestriction(typeof(IWeaponActorActions))] Object _actorWeaponActionsOverride;

        [Header("Weapon Skills Setting")]
        [SerializeField] [TypeRestriction(typeof(ISkill))] Object _normalAttackSkillObject;
        [SerializeField] [TypeRestriction(typeof(ISkill))] Object _specialAttackSkillObject;

        [System.Serializable]
        struct SkillActionBind
        {
            [TypeRestriction(typeof(ISkill))] 
            [SerializeField] Object _skillObject;
            [SerializeField] InputAction _skillPerformTrigger;

            ISkill _skill;
            public ISkill Skill => _skill ??= _skillObject.ConvertTo<ISkill>();
            public InputAction SkillPerformTrigger => _skillPerformTrigger;
        }
        [SerializeField] SkillActionBind[] _extraSkillBinds;

        public GameObject WeaponGameObject => gameObject;

        IActorController _owner;
        public IActorController Owner
        {
            get => _owner;
            set
            {
                _owner = value;

                EnableAllSkills();
                EnableAllPlayerInputActions();

                var weaponAnimationAdapter = GetComponent<IWeaponTiyaAnimatorAdapter>();
                weaponAnimationAdapter?.BindWeaponAnimation(Owner);
            }
        }

        IWeaponActorActions _defaultActions;
        public IWeaponActorActions DefaultActions => _defaultActions ??= new TiyaWeaponActorActions(this);

        IWeaponActorActions _weaponActions;
        public IWeaponActorActions ActorWeaponActions => _weaponActions ??= _actorWeaponActionsOverride != null ?
            _actorWeaponActionsOverride.ConvertTo<IWeaponActorActions>() : DefaultActions;

        ISkill _normalAttackSkill;
        public ISkill NormalAttackSkill => _normalAttackSkillObject == null ?
            null : (_normalAttackSkill ??= _normalAttackSkillObject.ConvertTo<ISkill>());

        ISkill _specialAttackSkill;
        public ISkill SpecialAttackSkill => _specialAttackSkillObject == null ?
            null : (_specialAttackSkill ??= _specialAttackSkillObject.ConvertTo<ISkill>());

        public IEnumerable<(ISkill skill, InputAction triggerAction)> ExtraSkillBinds => 
            from skillBind in _extraSkillBinds 
            select (skillBind.Skill, skillBind.SkillPerformTrigger);

        protected void Start()
        {
            RegisterAllPlayerInputActions();
        }

        protected void OnEnable()
        {
            EnableAllSkills();
            EnableAllPlayerInputActions();
        }

        protected void OnDisable()
        {
            DisableAllSkills();
            DisableAllPlayerInputActions();
        }

        private void EnableAllSkills()
        {
            if (Owner != null)
            {
                NormalAttackSkill?.Enable();
                SpecialAttackSkill?.Enable();
                foreach (var (skill, triggerAction) in ExtraSkillBinds)
                {
                    skill.Enable();
                    triggerAction.Enable();
                }
            }
        }
        private void DisableAllSkills()
        {
            if (Owner != null)
            {
                NormalAttackSkill?.Disable();
                SpecialAttackSkill?.Disable();
                foreach (var (skill, triggerAction) in ExtraSkillBinds)
                {
                    skill.Disable();
                    triggerAction.Disable();
                }
            }
        }

        private void RegisterAllPlayerInputActions()
        {
            var index = 0;
            foreach (var (_, trigger) in ExtraSkillBinds)
            {
                var skillAction = ActorWeaponActions.ExtraSkillTriggers[index];
                trigger.performed += _ => skillAction(SkillCmdType.PerformSkill);
                trigger.canceled += _ => skillAction(SkillCmdType.CancelSkill);
                index++;
            }
        }
        private void EnableAllPlayerInputActions()
        {
            if (Owner != null && Owner.IsPlayer)
            {
                foreach (var (_, trigger) in ExtraSkillBinds)
                {
                    trigger.Enable();
                }
            }
        }
        private void DisableAllPlayerInputActions()
        {
            if (Owner != null && Owner.IsPlayer)
            {
                foreach (var (_, trigger) in ExtraSkillBinds)
                {
                    trigger.Disable();
                }
            }
        }

        // 默认的 IWeaponActorActions。
        class TiyaWeaponActorActions : IWeaponActorActions
        {
            public TiyaWeaponController Weapon { get; }

            public IList<System.Action<SkillCmdType>> ExtraSkillTriggers { get; } = new List<System.Action<SkillCmdType>>();

            public void NormalAttack(SkillCmdType type)
            {
                switch (type)
                {
                    case SkillCmdType.PerformSkill:
                        Weapon.NormalAttackSkill?.TryToPerform();
                        break;
                    case SkillCmdType.CancelSkill:
                        Weapon.NormalAttackSkill?.Cancel();
                        break;
                }
            }

            public void SpecialAttack(SkillCmdType type)
            {
                switch (type)
                {
                    case SkillCmdType.PerformSkill:
                        Weapon.SpecialAttackSkill?.TryToPerform();
                        break;
                    case SkillCmdType.CancelSkill:
                        Weapon.SpecialAttackSkill?.Cancel();
                        break;
                }
            }

            public TiyaWeaponActorActions(TiyaWeaponController weapon)
            {
                Weapon = weapon;

                foreach (var (skill, _) in Weapon.ExtraSkillBinds)
                {
                    var action = new System.Action<SkillCmdType>(type =>
                    {
                        switch (type)
                        {
                            case SkillCmdType.PerformSkill:
                                skill.TryToPerform();
                                break;
                            case SkillCmdType.CancelSkill:
                                skill.Cancel();
                                break;
                        }
                    });
                    ExtraSkillTriggers.Add(action);
                }
            }
        }
    }
}
