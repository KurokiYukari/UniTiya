using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using Sarachan.UniTiya.Skill;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaWeapon
{
    /// <summary>
    /// 可配置的武器，通过组合 Skill 构造武器。
    /// ExtraSkills 可以定义对应的 InputAction；InputAction 只有在 Owner.IsPlayer 的情况下才会生效。
    /// 所有该武器的 Skill 和 InputAction 的 Enable 只会在改武器 Owner 已被设置的情况下发生，会在设置 Owner 时、组件 Enable 时调用；
    /// 而所有 Skil 和 InputAction 的 Disable 会在组件 Disable 时调用。
    /// <para>
    /// 注意：请不要将同一个 Skill 同时绑定在不同的技能槽内（比如同时作为 NormalAttak 和 SpecialAttack 的 Skill），这样可能会产生难以
    /// 预料的问题。（目前已知 <see cref="TiyaSimpleWeaponTiyaAnimationAdapter"/> 的动画绑定会出错，因为不同的动画参数设置 Listener 同时绑
    /// 定到了一个相同的 event 中）。
    /// </para>
    /// <para>
    /// TODO: 或者在 IWeaponController 中对于不同的武器行为添加独立的事件，以此来解决上面的问题？
    /// </para>
    /// </summary>
    /// TODO: 自动为其配置的 Skill 设置 SkillPerformer
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Tiya Weapon Controller")]
    public class TiyaWeaponController : MonoBehaviour, IWeaponController
    {
        [Tooltip("Unity Object or GameObject contains Component implements interface IWeaponActorActions.")]
        [SerializeField] [TypeRestriction(typeof(IWeaponActorActions))] Object _actorWeaponActionsOverride;

        [Header("Weapon Skills Setting")]
        [SerializeField] [TypeRestriction(typeof(ISkill))] Object _normalAttackSkillObject;
        [SerializeField] [TypeRestriction(typeof(ISkill))] Object _specialAttackSkillObject;

        [EditorOnly]
        [SerializeField] ActorAimMode _actorAimMode;

        TiyaWeaponControllerExtraSkillProvider[] _extraSkillProviders;
        TiyaWeaponControllerExtraSkillProvider[] ExtraSkillProviders => _extraSkillProviders ??= GetComponents<TiyaWeaponControllerExtraSkillProvider>();

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

                _owner.AimMode = _actorAimMode;

                OnEquip?.Invoke(Owner);
            }
        }

        IWeaponActorActions _defaultActions;
        public IWeaponActorActions DefaultActions => _defaultActions ??= new TiyaWeaponActorActions(this);

        IWeaponActorActions _weaponActions;
        public IWeaponActorActions ActorWeaponActions => _weaponActions ??= _actorWeaponActionsOverride != null ?
            _actorWeaponActionsOverride.ConvertTo<IWeaponActorActions>() : DefaultActions;

        ISkill _normalAttackSkill;
        public ISkill NormalSkill => _normalAttackSkillObject == null ?
            null : (_normalAttackSkill ??= _normalAttackSkillObject.ConvertTo<ISkill>());

        ISkill _specialAttackSkill;
        public ISkill SpecialSkill => _specialAttackSkillObject == null ?
            null : (_specialAttackSkill ??= _specialAttackSkillObject.ConvertTo<ISkill>());

        IReadOnlyList<(ISkill skill, InputAction triggerAction)> _extraSkillBindList;
        public IReadOnlyList<(ISkill skill, InputAction triggerAction)> ExtraSkillBinds => _extraSkillBindList ??=
            (from skillBind in ExtraSkillProviders
             select (skillBind.Skill, skillBind.SkillPerformTrigger)).ToList();

        IReadOnlyList<ISkill> _extraSkills;
        public IReadOnlyList<ISkill> ExtraSkills => _extraSkills ??=
            (from skillBind in ExtraSkillBinds
             select skillBind.skill).ToList();

        public event System.Action<IActorController> OnEquip;

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

        void EnableAllSkills()
        {
            if (Owner != null)
            {
                NormalSkill?.Enable();
                SpecialSkill?.Enable();
                foreach (var (skill, triggerAction) in ExtraSkillBinds)
                {
                    skill.Enable();
                    triggerAction?.Enable();
                }
            }
        }
        void DisableAllSkills()
        {
            if (Owner != null)
            {
                NormalSkill?.Disable();
                SpecialSkill?.Disable();
                foreach (var (skill, triggerAction) in ExtraSkillBinds)
                {
                    skill.Disable();
                    triggerAction?.Disable();
                }
            }
        }

        void RegisterAllPlayerInputActions()
        {
            var index = 0;
            foreach (var (_, trigger) in ExtraSkillBinds)
            {
                if (trigger != null)
                {
                    var localIndex = index;
                    trigger.performed += _ =>
                    {
                        ActorWeaponActions.ExtraSkillTriggers[localIndex](SkillCmdType.PerformSkill);
                    };
                    trigger.canceled += _ =>
                    {
                        ActorWeaponActions.ExtraSkillTriggers[localIndex](SkillCmdType.CancelSkill);
                    };
                }
                index++;
            }
        }
        void EnableAllPlayerInputActions()
        {
            if (Owner != null && Owner.IsPlayer)
            {
                foreach (var (_, trigger) in ExtraSkillBinds)
                {
                    trigger?.Enable();
                }
            }
        }
        void DisableAllPlayerInputActions()
        {
            if (Owner != null && Owner.IsPlayer)
            {
                foreach (var (_, trigger) in ExtraSkillBinds)
                {
                    trigger?.Disable();
                }
            }
        }

        // 默认的 IWeaponActorActions。
        class TiyaWeaponActorActions : IWeaponActorActions
        {
            public IWeaponController Weapon { get; }

            public IList<System.Action<SkillCmdType>> ExtraSkillTriggers { get; } = new List<System.Action<SkillCmdType>>();

            public void NormalAttack(SkillCmdType type)
            {
                switch (type)
                {
                    case SkillCmdType.PerformSkill:
                        Weapon.NormalSkill?.TryToPerform();
                        break;
                    case SkillCmdType.CancelSkill:
                        Weapon.NormalSkill?.Cancel();
                        break;
                }
            }

            public void SpecialAttack(SkillCmdType type)
            {
                switch (type)
                {
                    case SkillCmdType.PerformSkill:
                        Weapon.SpecialSkill?.TryToPerform();
                        break;
                    case SkillCmdType.CancelSkill:
                        Weapon.SpecialSkill?.Cancel();
                        break;
                }
            }

            public TiyaWeaponActorActions(IWeaponController weapon)
            {
                Weapon = weapon;

                foreach (var skill in Weapon.ExtraSkills)
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
