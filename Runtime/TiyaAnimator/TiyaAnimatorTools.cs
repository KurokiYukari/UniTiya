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
            public static int MaxAttackSegmentCount => 4;
            public static int MaxActionCount => 8;

            private static string LocomotionStateName => "Locomotion";
            private static string HoldNormalAttackStateName => "HoldNormalAttack Loop";

            private static string NormalStateNamePrefix => "NormalAttack";

            public static bool IsInLocomotionState(Animator tiyaAnimator)
            {
                var currentAnimatorState = tiyaAnimator.GetCurrentAnimatorStateInfo(Layer.WeaponFullBody.ToLayerIndex());

                return currentAnimatorState.IsName(LocomotionStateName);
            }

            public static bool IsInNormalAttackStates(Animator tiyaAnimator)
            {
                var currentAnimatorState = tiyaAnimator.GetCurrentAnimatorStateInfo(Layer.WeaponFullBody.ToLayerIndex());

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
            public static string AnimationSpeed_F => "AnimationSpeed";
            public static string IsGround_B => "IsGround";
            public static string Jump_T => "JumpTrigger";
            public static string ScaledSpeed_F => "ScaledSpeed";
            public static string DirectionX_F => "DirectionX";
            public static string DirectionZ_F => "DirectionZ";
            public static string NormalAttackTrigger_T => "NormalAttackTrigger";
            public static string NormalAttackSegmentCount_I => "NormalAttackSegmentCount";
            public static string HoldNormalAttack_B => "HoldNormalAttack";
            public static string SpecialAttackTrigger_T => "SpecialAttackTrigger";
            public static string HoldSpecialAttack_B => "HoldSpecialAttack";
            public static string SpecialAttackSegmentCount_I => "SpecialAttackSegmentCount";
            public static string StopHoldAttackTrigger_T => "StopHoldAttackTrigger";

            /// <summary>
            /// 0 代表无，1 - 8 对应 8 种不同的动画。
            /// 默认大于 8 的行为无意义。
            /// </summary>
            public static string WeaponActionType_I => "WeaponActionType";
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
        }

        /// <summary>
        /// 将 Layer enum 转化为 TiyaAnimator 中的层 index
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int ToLayerIndex(this Layer layer) => (int)layer / 2;

        /// <summary>
        /// 将 TiyaAnimator 中指定层的权重进行一次插值到目标权重
        /// </summary>
        /// <param name="tiyaAnimator"></param>
        /// <param name="tiyaAnimatorLayer"></param>
        /// <param name="destinateWeight"></param>
        /// <param name="lerpRatio"></param>
        public static void SingleLayerWeightLerper(Animator tiyaAnimator, Layer tiyaAnimatorLayer, float destinateWeight = 1, float lerpRatio = 1f) =>
            tiyaAnimator.SetLayerWeight(tiyaAnimatorLayer.ToLayerIndex(), Mathf.Lerp(tiyaAnimator.GetLayerWeight(tiyaAnimatorLayer.ToLayerIndex()), destinateWeight, lerpRatio));

        /// <summary>
        /// 将 TiyaAnimator 中的所有层的权重按照 Layer 对应位是否设置进行一次插值到权重 1
        /// </summary>
        /// <param name="tiyaAnimator"></param>
        /// <param name="tiyaAnimatorLayers"></param>
        /// <param name="lerpRatio"></param>
        public static void AllLayerWeightLerper(Animator tiyaAnimator, Layer tiyaAnimatorLayers, float lerpRatio = 3f)
        {
            foreach (Layer layer in System.Enum.GetValues(typeof(Layer)))
            {
                if ((layer & tiyaAnimatorLayers) != 0)
                {
                    tiyaAnimator.SetLayerWeight(layer.ToLayerIndex(), Mathf.Lerp(tiyaAnimator.GetLayerWeight(layer.ToLayerIndex()), 1, lerpRatio * Time.deltaTime));
                }
                else
                {
                    tiyaAnimator.SetLayerWeight(layer.ToLayerIndex(), Mathf.Lerp(tiyaAnimator.GetLayerWeight(layer.ToLayerIndex()), 0, lerpRatio * Time.deltaTime));
                }
            }
        }
    }
}
