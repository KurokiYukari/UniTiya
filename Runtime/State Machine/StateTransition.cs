using System;
using System.Collections.Generic;

namespace Sarachan.UniTiya.StateMachine
{
    /// <summary>
    /// State 的转换
    /// </summary>
    public sealed class StateTransition
    {
        /// <summary>
        /// 转换目标 State 的 ID，null 代表退出当前状态，转向 StateMachine 的 EntryState。
        /// </summary>
        public string DestinationStateId { get; }

        public event Action OnTransitionDone;

        readonly Func<bool> _triggerCondition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationStateId">目标 State。默认是退出当前 State，交由状态机决定目标 State</param>
        /// <param name="triggerCondition">转换条件。默认是无条件转换</param>
        public StateTransition(string destinationStateId = null, Func<bool> triggerCondition = null, Action onTransitionDone = null)
        {
            DestinationStateId = destinationStateId;

            _triggerCondition = triggerCondition;

            if (onTransitionDone != null)
            {
                OnTransitionDone += onTransitionDone;
            }
        }

        /// <summary>
        /// 检测当前 transition 是否满足转换条件
        /// 应该在 State 的 OnStateUpdate 中检测。
        /// </summary>
        /// <returns></returns>
        public bool CheckTransition()
        {
            if (_triggerCondition == null)
            {
                return true;
            }

            return _triggerCondition();
        }

        /// <summary>
        /// 触发 Transition 回调
        /// </summary>
        public void DoTransition() => OnTransitionDone?.Invoke();
    }
}
