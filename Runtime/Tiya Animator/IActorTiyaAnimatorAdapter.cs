using UnityEngine;

namespace Sarachan.UniTiya.TiyaAnimator
{
    /// <summary>
    /// TiyaAnimator 对于 IActorController 的适配器接口
    /// </summary>
    public interface IActorTiyaAnimatorAdapter
    {
        // Actor Animator Adapter 中基础的动画建议直接使用 AnimatorOverrideController配置
        // TODO: 额外动作动画？

        RuntimeAnimatorController OriginAnimatorController { get; }
        IEnable LeftHandIK { get; }
        Transform LeftHandIKHandler { get; set; }
    }
}
