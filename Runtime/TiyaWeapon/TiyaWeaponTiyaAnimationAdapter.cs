using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaAnimator;

namespace Sarachan.UniTiya.TiyaWeapon
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Weapon Tiya Animator Adapter")]
    public class TiyaWeaponTiyaAnimationAdapter : WeaponTiyaAnimatorAdapterBase
    {
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

        public void BindToWeapon(TiyaWeaponController weapon)
        {
            if (!NormalAttackAnimationBind.IsEmpty() && weapon.NormalAttackSkill != null)
            {
                weapon.NormalAttackSkill.OnPerforming += NormalAttackAnimationBind.OnDoAttack;
            }
            if (!SpecialAttackAnimationBind.IsEmpty() && weapon.SpecialAttackSkill != null)
            {
                weapon.SpecialAttackSkill.OnPerforming += SpecialAttackAnimationBind.OnDoAttack;
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
        }

        private void Start()
        {
            BindToWeapon(Weapon);
        }
    }
}
