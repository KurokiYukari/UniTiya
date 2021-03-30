using UnityEngine;

using UniRx;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// Actor 的游戏属性
    /// </summary>
    public interface IActorProperties : IGameProperties
    {
        string ActorName { get; }
        IGameDynamicNumericalProperty ActorHP { get; }
        IGameFixedNumericalProperty WalkSpeed { get; }
        IGameFixedNumericalProperty RunSpeed { get; }
        IGameFixedNumericalProperty SprintSpeed { get; }
        IGameFixedNumericalProperty JumpInitialSpeed { get; }
    }

    public static partial class TiyaTools
    {
        public enum HealthValueType 
        {
            /// <summary>
            /// 恢复固定的数值
            /// </summary>
            FixedValue,
            /// <summary>
            /// 按照最大 HP 百分比恢复
            /// </summary>
            PercentOfMaxValue,
            /// <summary>
            /// 按照已损失 HP 百分比恢复
            /// </summary>
            PercentOfLosingValue
        }

        /// <summary>
        /// 瞬时恢复 HP
        /// </summary>
        /// <param name="actorProperties"></param>
        /// <param name="healthValue"></param>
        /// <param name="healthValueType"></param>
        public static void InstantHealth(this IActorProperties actorProperties, float healthValue, HealthValueType healthValueType)
        {
            var actorHPReference = actorProperties.ActorHP;
            var healthFloat = healthValueType switch
            {
                HealthValueType.FixedValue => healthValue,
                HealthValueType.PercentOfMaxValue => healthValue * actorHPReference.MaxValue / 100,
                HealthValueType.PercentOfLosingValue => healthValue * (actorHPReference.MaxValue - actorHPReference.Value) / 100,
                _ => throw new System.InvalidOperationException(),
            };
            actorHPReference.Value += healthFloat;
        }

        /// <summary>
        /// 在一定时间内持续恢复 HP
        /// </summary>
        /// <param name="actorProperties"></param>
        /// <param name="healthValue">恢复的总 HP 值</param>
        /// <param name="healthValueType"></param>
        /// <param name="healthDuration">恢复效果持续时间</param>
        public static void ContinuousHealth(this IActorProperties actorProperties, float healthValue, HealthValueType healthValueType, float healthDuration)
        {
            var actorHPReference = actorProperties.ActorHP;
            var healthFloat = healthValueType switch
            {
                HealthValueType.FixedValue => healthValue,
                HealthValueType.PercentOfMaxValue => healthValue * actorHPReference.MaxValue / 100,
                HealthValueType.PercentOfLosingValue => healthValue * (actorHPReference.MaxValue - actorHPReference.Value) / 100,
                _ => throw new System.InvalidOperationException(),
            };
            Observable.EveryUpdate()
                    .TakeUntil(Observable.Timer(System.TimeSpan.FromSeconds(healthDuration)))
                    .Subscribe(_ => actorHPReference.Value += healthFloat * Time.deltaTime);
        }
    }
}
