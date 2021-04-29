using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaAnimator;

namespace Sarachan.UniTiya.TiyaWeapon
{
    /// <summary>
    /// 仅在装备武器时将 actor 原本的 AnimatorController 替换为指定 AnimatorController。但注意，参数设
    /// 置、层转换等都未被处理。
    /// 建议：该 AnimatorController 复制与 TiyaAnimatorController，只在 Basic\FullBody 层添加新的状态或
    /// 直接添加新的层（因为 Basic 层与 ActorAdapter 相关所以不要修改，Weapon 层如果不使用可以直接删去）
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Weapon Tiya Animator Overrider")]
    public class TiyaWeaponTiyaAnimatorOverrider : MonoBehaviour
    {
        public IWeaponController Weapon { get; private set; }

        [SerializeField] RuntimeAnimatorController _overrideAnimatorController;

        [SerializeField] bool _setDefaultTiyaAnimatorParamTriggers = true;

        protected void Awake()
        {
            Weapon = GetComponent<IWeaponController>() ?? throw new MissingComponentException(nameof(IWeaponController));

            if (!_overrideAnimatorController)
            {
                throw new MissingReferenceException(nameof(_overrideAnimatorController));
            }

            Weapon.OnEquip += SetAnimatorListener;

            if (_setDefaultTiyaAnimatorParamTriggers)
            {
                Weapon.OnEquip += SetAnimatorParamTriggerListener;
            }

            void SetAnimatorListener(IActorController owner)
            {
                owner.Animator.runtimeAnimatorController = _overrideAnimatorController;
            }
            void SetAnimatorParamTriggerListener(IActorController owner)
            {
                if (Weapon.NormalSkill != null)
                {
                    Weapon.NormalSkill.OnPerforming += () => owner.Animator.SetTrigger(TiyaAnimatorTools.Params.NormalAttackTrigger_T);
                }
                if (Weapon.SpecialSkill != null)
                {
                    Weapon.SpecialSkill.OnPerforming += () => owner.Animator.SetTrigger(TiyaAnimatorTools.Params.SpecialAttackTrigger_T);
                }
                for (int i = 0; i < Weapon.ExtraSkills.Count; i++)
                {
                    Weapon.ExtraSkills[i].OnPerforming += () => owner.Animator.SetInteger(TiyaAnimatorTools.Params.WeaponActionType_I, i);
                }
            }
        }
    }
}
