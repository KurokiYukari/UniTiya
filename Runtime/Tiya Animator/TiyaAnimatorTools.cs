using System.Collections;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaAnimator
{
    /// <summary>
    /// TiyaAnimator 的工具类
    /// </summary>
    public static partial class TiyaAnimatorTools
    {
        /// <summary>
        /// 武器层的信息于各种工具方法
        /// </summary>
        public static class WeaponLayerStates
        {
            public const int MaxAttackSegmentCount = 4;
            public const int MaxActionCount = 8;

            private const string LocomotionStateName = "Locomotion";
            private const string HoldNormalAttackStateName = "HoldNormalAttack Loop";

            private const string NormalStateNamePrefix = "NormalAttack";

            public static bool IsInLocomotionState(Animator tiyaAnimator)
            {
                var currentAnimatorState = tiyaAnimator.GetCurrentAnimatorStateInfo(Layer.WeaponFullBody.ToLayerIndex(tiyaAnimator));

                return currentAnimatorState.IsName(LocomotionStateName);
            }

            public static bool IsInNormalAttackStates(Animator tiyaAnimator)
            {
                var currentAnimatorState = tiyaAnimator.GetCurrentAnimatorStateInfo(Layer.WeaponFullBody.ToLayerIndex(tiyaAnimator));

                bool result = false;
                for (int i = 1; i <= MaxAttackSegmentCount; i++)
                {
                    result = result || currentAnimatorState.IsName($"{NormalStateNamePrefix} {i}");
                }
                result = result || currentAnimatorState.IsName(HoldNormalAttackStateName);

                return result;
            }
        }

        /// <summary>
        /// Tiya Animator Controller 的所有默认参数名
        /// </summary>
        public static class Params
        {
            // Base Layer
            public const string AnimationSpeed_F = "AnimationSpeed";
            public const string IsGround_B = "IsGround";
            public const string Jump_T = "JumpTrigger";
            public const string ScaledSpeed_F = "ScaledSpeed";
            public const string DirectionX_F = "DirectionX";
            public const string DirectionZ_F = "DirectionZ";

            // Weapon Layer
            public const string NormalAttackTrigger_T = "NormalAttackTrigger";
            public const string NormalAttackSegmentCount_I = "NormalAttackSegmentCount";
            public const string HoldNormalAttack_B = "HoldNormalAttack";
            public const string SpecialAttackTrigger_T = "SpecialAttackTrigger";
            public const string HoldSpecialAttack_B = "HoldSpecialAttack";
            public const string SpecialAttackSegmentCount_I = "SpecialAttackSegmentCount";
            public const string StopHoldAttackTrigger_T = "StopHoldAttackTrigger";

            /// <summary>
            /// 0 代表无，1 - 8 对应 8 种不同的动画。
            /// 默认大于 8 的行为无意义。
            /// </summary>
            public const string WeaponActionType_I = "WeaponActionType";

            // FullBody Layer
            public const string IsAlive_B = "IsAlive";
        }

        /// <summary>
        /// Tiya Animator Controller 的所有层。
        /// 可以通过与同时指定多层。可以通过 ToLayerIndex 将之转换为层索引
        /// </summary>
        public enum Layer
        {
            Base = 1,
            WeaponUpperBody = 1 << 1,
            WeaponFullBody = 1 << 2,
            FullBody = 1 << 3,
        }

        const string BASE_LAYER_NAME = "Base Layer";
        const string WEAPON_UPPER_BODY_LAYER_NAME = "WeaponUpperBody Layer";
        const string WEAPON_FULL_BODY_NAME = "WeaponFullBody Layer";
        const string FULL_BODY_LAYER = "FullBody Layer";

        public static string ToLayerName(this Layer layer) =>
            layer switch
            {
                Layer.Base => BASE_LAYER_NAME,
                Layer.WeaponUpperBody => WEAPON_UPPER_BODY_LAYER_NAME,
                Layer.WeaponFullBody => WEAPON_FULL_BODY_NAME,
                Layer.FullBody => FULL_BODY_LAYER,
                _ => throw new System.InvalidOperationException($"Layer {layer} don't exist.")
            };

        /// <summary>
        /// 将 Layer enum 转化为 TiyaAnimator 中的层 index。
        /// 对于复合的 layer，将会返回 -1
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int ToLayerIndex(this Layer layer, Animator animator) =>
            animator.GetLayerIndex(layer.ToLayerName());

        /// <summary>
        /// 将 TiyaAnimator 中指定层的权重进行一次插值到目标权重
        /// </summary>
        /// <param name="tiyaAnimator"></param>
        /// <param name="tiyaAnimatorLayer"></param>
        /// <param name="destinateWeight"></param>
        /// <param name="lerpRatio"></param>
        public static void SingleLayerWeightLerper(Animator tiyaAnimator, Layer tiyaAnimatorLayer, float destinateWeight = 1, float lerpRatio = 1f) =>
            tiyaAnimator.SetLayerWeight(tiyaAnimatorLayer.ToLayerIndex(tiyaAnimator), Mathf.Lerp(tiyaAnimator.GetLayerWeight(tiyaAnimatorLayer.ToLayerIndex(tiyaAnimator)), destinateWeight, lerpRatio));

        /// <summary>
        /// 将 TiyaAnimator 中的层的权重按照 Layer 对应位是否设置进行一次插值到权重 1
        /// </summary>
        /// <param name="tiyaAnimator"></param>
        /// <param name="tiyaAnimatorLayers">要进行插值的复合层</param>
        /// <param name="maxLayer">受影响的最高层。比 maxLayer 高的层不会被更改。默认所有层都会被改变</param>
        /// <param name="lerpRatio"></param>
        public static void AllLayerWeightLerper(Animator tiyaAnimator, Layer tiyaAnimatorLayers, Layer? maxLayer = null, float lerpRatio = 3f)
        {
            var maxLayerIndex = int.MaxValue;
            if (maxLayer.HasValue)
            {
                maxLayerIndex = maxLayer.Value.ToLayerIndex(tiyaAnimator);
            }

            foreach (Layer layer in System.Enum.GetValues(typeof(Layer)))
            {
                if (layer.ToLayerIndex(tiyaAnimator) <= maxLayerIndex)
                {
                    if ((layer & tiyaAnimatorLayers) != 0)
                    {
                        tiyaAnimator.SetLayerWeight(layer.ToLayerIndex(tiyaAnimator), Mathf.Lerp(tiyaAnimator.GetLayerWeight(layer.ToLayerIndex(tiyaAnimator)), 1, lerpRatio * Time.deltaTime));
                    }
                    else
                    {
                        tiyaAnimator.SetLayerWeight(layer.ToLayerIndex(tiyaAnimator), Mathf.Lerp(tiyaAnimator.GetLayerWeight(layer.ToLayerIndex(tiyaAnimator)), 0, lerpRatio * Time.deltaTime));
                    }
                }
            }
        }
    }
}
