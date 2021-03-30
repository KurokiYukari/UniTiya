using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaAnimator
{
    /// <summary>
    /// 实现了 IWeaponTiyaAnimatorAdapter 的基 Behaviour，他已经处理了权重的配置
    /// </summary>
    public abstract class WeaponTiyaAnimatorAdapterBase : MonoBehaviour, IWeaponTiyaAnimatorAdapter
    {
        protected IWeaponController Weapon { get; set; }
        protected Animator OwnerAnimator => Weapon.Owner.Animator;
        protected TiyaAnimatorTools.Layer CurrentTiyaAnimatorLayer { get; set; }

        public abstract WeaponLocomotionAnimationBind LocomotionAnimationBind { get; }
        public abstract WeaponAttackAnimationBind NormalAttackAnimationBind { get; }
        public abstract WeaponAttackAnimationBind SpecialAttackAnimationBind { get; }
        public abstract IEnumerable<WeaponActionAnimationBind> ExtraActionAnimationBinds { get; }

        private List<WeaponActionAnimationBind> _extraActionAnimationBindList = null;
        private List<WeaponActionAnimationBind> ExtraActionAnimationBindList
        {
            get
            {
                if (_extraActionAnimationBindList == null && ExtraActionAnimationBinds != null)
                {
                    _extraActionAnimationBindList = ExtraActionAnimationBinds.ToList();
                }
                return _extraActionAnimationBindList;
            }
        }

        protected void Awake()
        {
            Weapon = GetComponent<IWeaponController>();
        }

        protected void Update()
        {
            if (Weapon.Owner != null)
            {
                TiyaAnimatorTools.AllLayerWeightLerper(OwnerAnimator, CurrentTiyaAnimatorLayer);

                if (TiyaAnimatorTools.WeaponLayerStates.IsInLocomotionState(OwnerAnimator))
                {
                    CurrentTiyaAnimatorLayer = LocomotionAnimationBind.Layer;
                }

                if (TiyaAnimatorTools.WeaponLayerStates.IsInNormalAttackStates(OwnerAnimator))
                {
                    CurrentTiyaAnimatorLayer = NormalAttackAnimationBind.Layer;
                }

                for (int i = 0; i < ExtraActionAnimationBindList.Count; i++)
                {
                    // TODO: 这里会有延时，导致层的切换慢于动画的开始
                    if (OwnerAnimator.GetCurrentAnimatorStateInfo(TiyaAnimatorTools.Layer.WeaponFullBody.ToLayerIndex()).IsName($"Action.Action {i + 1}"))
                    {
                        CurrentTiyaAnimatorLayer = ExtraActionAnimationBindList[i].Layer;
                        OwnerAnimator.SetLayerWeight(CurrentTiyaAnimatorLayer.ToLayerIndex(), 1);
                    }
                }
            }
        }
    }
}
