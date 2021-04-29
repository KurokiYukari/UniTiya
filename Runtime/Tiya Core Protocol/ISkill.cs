using System;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// Skill 接口。Skill 是对行为的一种抽象，它会尝试去执行某种行为（但不一定成功）。
    /// Skill 支持两种行为：
    /// <list type="bullet">
    /// 瞬时行为：行为的所有逻辑全在 <see cref="TryToPerform"/> 中实现，<see cref="Cancel"/> 对行为无影响。
    /// </list>
    /// <list type="bullet">
    /// 持续行为：行为会在 <see cref="TryToPerform"/> 返回 true 时开始，<see cref="Cancel"/> 时结束。
    /// <see cref="Cancel"/> 只有在行为开始状态才有实际意义，其余状态对行为无影响。
    /// </list>
    /// </summary>
    public interface ISkill : IEnable
    {
        // TODO: 添加 IActorController 的 SkillPerformer？

        event Action OnPerforming;
        event Action OnPerformingFailed;
        event Action OnCanceling;

        /// <summary>
        /// 尝试使用该技能。
        /// </summary>
        /// <returns>使用成功返回 true，否则返回 false</returns>
        bool TryToPerform();

        /// <summary>
        /// 取消当前正在使用的行为
        /// </summary>
        void Cancel();
    }
}
