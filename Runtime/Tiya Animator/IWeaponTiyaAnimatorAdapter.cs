using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Sarachan.UniTiya.TiyaAnimator
{
    /// <summary>
    /// TiyaAnimator 对于 IWeaponController 的接口
    /// </summary>
    public interface IWeaponTiyaAnimatorAdapter
    {
        /// <summary>
        /// 移动动画绑定
        /// </summary>
        WeaponLocomotionAnimationBind LocomotionAnimationBind { get; }

        // 两个攻击绑定（默认是左键和右键
        WeaponAttackAnimationBind NormalAttackAnimationBind { get; }
        WeaponAttackAnimationBind SpecialAttackAnimationBind { get; }

        /// <summary>
        /// 自定义行为动画绑定
        /// </summary>
        IEnumerable<WeaponActionAnimationBind> ExtraActionAnimationBinds { get; }
    }

    public static partial class TiyaAnimatorTools
    {
        /// <summary>
        /// 将 Weapon 动画绑定在 actor 的 TiyaAnimator 上
        /// </summary>
        /// <param name="weaponAnimationProvider"></param>
        /// <param name="actor"></param>
        public static void BindWeaponAnimation(this IWeaponTiyaAnimatorAdapter weaponAnimationProvider, IActorController actor)
        {
            var animator = actor.Animator;
            var actorAnimatorAdapter = actor.GameObject.GetComponent<IActorTiyaAnimatorAdapter>();
            if (actorAnimatorAdapter == null)
            {
                throw new MissingComponentException($"IK Setting needs {nameof(IActorTiyaAnimatorAdapter)} Component in weapon's owner, but it is missing.");
            }
            var animatorController = new AnimatorOverrideController(actorAnimatorAdapter.OriginAnimatorController);
            animator.runtimeAnimatorController = animatorController;

            // bind locomotion
            string layerPrefix = null;
            string clipName = null;
            if (weaponAnimationProvider.LocomotionAnimationBind.Layer == Layer.WeaponUpperBody)
                layerPrefix = "WeaponUpperBody";
            else if (weaponAnimationProvider.LocomotionAnimationBind.Layer == Layer.WeaponFullBody)
                layerPrefix = "WeaponFullBody";

            if (layerPrefix != null)
            {
                if (weaponAnimationProvider.LocomotionAnimationBind.IdleAnimation != null)
                {
                    clipName = $"{layerPrefix}Idle";
                    animatorController[clipName] = weaponAnimationProvider.LocomotionAnimationBind.IdleAnimation;
                }

                if (weaponAnimationProvider.LocomotionAnimationBind.WalkAnimation != null)
                {
                    animatorController[$"{layerPrefix}Walk"] = weaponAnimationProvider.LocomotionAnimationBind.WalkAnimation;
                }

                if (weaponAnimationProvider.LocomotionAnimationBind.RunAnimation != null)
                {
                    animatorController[$"{layerPrefix}Run"] = weaponAnimationProvider.LocomotionAnimationBind.RunAnimation;
                }
            }

            // bind normal attack
            var normalAttackAnimationBind = weaponAnimationProvider.NormalAttackAnimationBind;
            if (!normalAttackAnimationBind.IsEmpty())
            {
                animator.SetBool(Params.HoldNormalAttack_B, normalAttackAnimationBind.IsHoldLoop);
                animator.SetInteger(Params.NormalAttackSegmentCount_I, normalAttackAnimationBind.AttackSegmentCount);
                normalAttackAnimationBind.OnDoAttack += () => animator.SetTrigger(Params.NormalAttackTrigger_T);

                layerPrefix = null;
                if (normalAttackAnimationBind.Layer == Layer.WeaponUpperBody)
                    layerPrefix = "WeaponUpperBody";
                else if (normalAttackAnimationBind.Layer == Layer.WeaponFullBody)
                    layerPrefix = "WeaponFullBody";

                if (layerPrefix != null)
                {
                    if (normalAttackAnimationBind.IsHoldLoop)
                    {
                        animatorController[$"{layerPrefix} HoldNormalAttack Loop"] = normalAttackAnimationBind.AnimationList[0];
                    }
                    else
                    {
                        for (int i = 0; i < normalAttackAnimationBind.AttackSegmentCount; i++)
                        {
                            clipName = $"{layerPrefix}NormalAttack {i + 1}";
                            animatorController[clipName] = normalAttackAnimationBind.AnimationList[i];
                        }
                    }
                }
            }

            // bind special attack
            var specialAttackAnimationBind = weaponAnimationProvider.SpecialAttackAnimationBind;
            if (!specialAttackAnimationBind.IsEmpty())
            {
                animator.SetInteger(Params.SpecialAttackSegmentCount_I, specialAttackAnimationBind.AttackSegmentCount);
                animator.SetBool(Params.HoldSpecialAttack_B, specialAttackAnimationBind.IsHoldLoop);
                specialAttackAnimationBind.OnDoAttack += () => animator.SetTrigger(Params.SpecialAttackTrigger_T);

                layerPrefix = null;
                if (specialAttackAnimationBind.Layer == Layer.WeaponUpperBody)
                    layerPrefix = "WeaponUpperBody";
                else if (specialAttackAnimationBind.Layer == Layer.WeaponFullBody)
                    layerPrefix = "WeaponFullBody";

                if (layerPrefix != null)
                {
                    if (specialAttackAnimationBind.IsHoldLoop)
                    {
                        animatorController[$"{layerPrefix} HoldSpecialAttack Loop"] = specialAttackAnimationBind.AnimationList[0];
                    }
                    else
                    {
                        for (int i = 0; i < specialAttackAnimationBind.AttackSegmentCount; i++)
                        {
                            animatorController[$"{layerPrefix}SpecialAttack {i + 1}"] = specialAttackAnimationBind.AnimationList[i];
                        }
                    }
                }
            }

            // bind extra actions
            int upperBodyCount = 0, fullBodyCount = 0;
            var extraActionBinds = weaponAnimationProvider.ExtraActionAnimationBinds;
            if (extraActionBinds != null)
            {
                foreach (var actionBind in extraActionBinds)
                {
                    if (actionBind.IsEmpty())
                    {
                        return;
                    }

                    if (actionBind.Layer == Layer.WeaponUpperBody && ++upperBodyCount <= 8)
                    {
                        animatorController[$"WeaponUpperBodyAction {upperBodyCount}"] = actionBind.Animation;
                        actionBind.OnDoAction += (() => animator.SetInteger(Params.WeaponActionType_I, upperBodyCount));
                        actionBind.AnimtatorStateName = $"Action {upperBodyCount}";
                    }
                    else if (actionBind.Layer == Layer.WeaponFullBody && ++fullBodyCount <= 8)
                    {
                        animatorController[$"WeaponFullBodyAction {fullBodyCount}"] = actionBind.Animation;
                        actionBind.OnDoAction += (() => animator.SetInteger(Params.WeaponActionType_I, fullBodyCount));
                        actionBind.AnimtatorStateName = $"Action {fullBodyCount}";
                    }
                    else
                    {
                        Debug.LogWarning($"Bind WeaponAction Animation: {actionBind.Animation.name} Failed");
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct WeaponLocomotionAnimationBind
    {
        [SerializeField] private AnimationClip _idleAnimation;
        [SerializeField] private AnimationClip _walkAnimation;
        [SerializeField] private AnimationClip _runAnimation;
        [SerializeField] private TiyaAnimatorTools.Layer _layer;

        public AnimationClip IdleAnimation => _idleAnimation;
        public AnimationClip WalkAnimation => _walkAnimation;
        public AnimationClip RunAnimation => _runAnimation;
        public TiyaAnimatorTools.Layer Layer => _layer;

        public bool IsEmpty() => _idleAnimation == null && _walkAnimation == null && _runAnimation == null;

        public WeaponLocomotionAnimationBind(AnimationClip idleAnimation, AnimationClip walkAnimation, AnimationClip runAnimation, TiyaAnimatorTools.Layer layer)
        {
            _idleAnimation = idleAnimation;
            _walkAnimation = walkAnimation;
            _runAnimation = runAnimation;
            _layer = layer;
        }
    }

    [System.Serializable]
    public class WeaponAttackAnimationBind
    {
        [SerializeField] private bool _isHoldLoop;
        [SerializeField] private List<AnimationClip> _animationList;
        [SerializeField] private TiyaAnimatorTools.Layer _layer;

        public System.Action OnDoAttack { get; set; }

        public int AttackSegmentCount => _animationList == null ? 0 : _animationList.Count;
        public bool IsHoldLoop => AttackSegmentCount > 0 && _isHoldLoop;
        public List<AnimationClip> AnimationList => _animationList;
        public TiyaAnimatorTools.Layer Layer => _layer;

        public bool IsEmpty() => AttackSegmentCount == 0;

        public WeaponAttackAnimationBind(bool isHoldLoop = false, List<AnimationClip> animationList = null, TiyaAnimatorTools.Layer layer = TiyaAnimatorTools.Layer.Base)
        {
            _isHoldLoop = isHoldLoop;
            _animationList = animationList;
            _layer = layer;
        }
    }

    [System.Serializable]
    public class WeaponActionAnimationBind
    {
        [SerializeField] private AnimationClip _animation;
        [SerializeField] private TiyaAnimatorTools.Layer _layer;

        public string AnimtatorStateName { get; set; }
        public System.Action OnDoAction { get; set; }

        public AnimationClip Animation => _animation;
        public TiyaAnimatorTools.Layer Layer => _layer;

        public bool IsEmpty() => _animation == null;
    }
}
