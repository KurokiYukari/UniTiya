using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaAnimator;

namespace Sarachan.UniTiya.TiyaWeapon
{
    // TODO: 添加直接修改状态机的 AnimationAdapter 以适应更加复杂的要求
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Simple Weapon Tiya Animator Adapter")]
    public class TiyaSimpleWeaponTiyaAnimationAdapter : WeaponTiyaAnimatorAdapterBase
    {
        [SerializeField] Transform _leftHandIKHandler;

        [SerializeField] private WeaponLocomotionAnimationBind _locomotionAnimationBind;
        [SerializeField] private WeaponAttackAnimationBind _normalAttackAnimationBind;
        [SerializeField] private WeaponAttackAnimationBind _specialAttackAnimationBind;
        
        [Tooltip("Each AnimationBind matches a Skill in TiyaWeaponController's ExtraSkills by index.")]
        [SerializeField] private List<WeaponActionAnimationBind> _extraActionAnimationBind;

        public override WeaponLocomotionAnimationBind LocomotionAnimationBind => _locomotionAnimationBind;
        public override WeaponAttackAnimationBind NormalAttackAnimationBind => _normalAttackAnimationBind;
        public override WeaponAttackAnimationBind SpecialAttackAnimationBind => _specialAttackAnimationBind;
        public override IEnumerable<WeaponActionAnimationBind> ExtraActionAnimationBinds => _extraActionAnimationBind;

        public new TiyaWeaponController Weapon => base.Weapon as TiyaWeaponController;

        // TODO: 把这个方法放在 IWeaponTiyaAnimatorAdapter.BindWeaponAnimation 里？
        // 目前要保证该方法调用晚于 IWeaponTiyaAnimatorAdapter.BindWeaponAnimation
        public void InitWeaponAnimation(IActorController weaponOwner)
        {
            var weapon = Weapon;

            if (!NormalAttackAnimationBind.IsEmpty() && weapon.NormalSkill != null)
            {
                weapon.NormalSkill.OnPerforming += NormalAttackAnimationBind.OnDoAttack;

                weapon.NormalSkill.OnCanceling += () => CancelSkillListener(NormalAttackAnimationBind.IsHoldLoop);
            }
            if (!SpecialAttackAnimationBind.IsEmpty() && weapon.SpecialSkill != null)
            {
                weapon.SpecialSkill.OnPerforming += SpecialAttackAnimationBind.OnDoAttack;

                weapon.SpecialSkill.OnCanceling += () => CancelSkillListener(SpecialAttackAnimationBind.IsHoldLoop);
            }

            var extraSkillEnumerator = weapon.ExtraSkillBinds.GetEnumerator();
            foreach (var animationBind in ExtraActionAnimationBinds)
            {
                if (!extraSkillEnumerator.MoveNext())
                {
                    Debug.LogError($"{nameof(ExtraActionAnimationBinds)}'s count cann't be more than {nameof(weapon.name)}.{nameof(weapon.ExtraSkillBinds)}'s count.");
                    break;
                }
                var skill = extraSkillEnumerator.Current.skill;

                if (!animationBind.IsEmpty())
                {
                    skill.OnPerforming += animationBind.OnDoAction;
                }
            }

            var ownerAnimatorAdapter = weaponOwner.GameObject.GetComponent<IActorTiyaAnimatorAdapter>();
            if (ownerAnimatorAdapter != null)
            {
                ownerAnimatorAdapter.LeftHandIKHandler = _leftHandIKHandler;
            }
            else
            {
                Debug.LogWarning($"IK Setting needs {nameof(IActorTiyaAnimatorAdapter)} Component in weapon's owner, but it is missing.");
            }

            void CancelSkillListener(bool isHoldLoop)
            {
                if (isHoldLoop)
                {
                    weaponOwner.Animator.SetTrigger(TiyaAnimatorTools.Params.StopHoldAttackTrigger_T);
                }
            }
        }

        protected new void Awake()
        {
            base.Awake();

            Weapon.OnEquip += this.BindWeaponAnimation;
            Weapon.OnEquip += InitWeaponAnimation;
        }
    }
}
