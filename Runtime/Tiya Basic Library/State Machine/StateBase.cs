using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.StateMachine
{
    /// <summary>
    /// 状态基类，通过配置其事件来自定义状态
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// State 所属的 StateMachine
        /// </summary>
        public TiyaStateMachine StateMachine { get; }

        /// <summary>
        /// State 所属的 StateGroup
        /// </summary>
        public StateBase StateGroup { get; }

        /// <summary>
        /// State 控制的主体对象
        /// </summary>
        public object Subject => StateMachine.Subject;

        readonly IList<StateTransition> _transitions = new List<StateTransition>();

        /// <summary>
        /// State 的所有可能的转换
        /// 转换是有序的，会选择第一个满足条件的转换执行
        /// </summary>
        public IEnumerable<StateTransition> AllTransitionsEnumerable
        {
            get
            {
                foreach (var transition in _transitions)
                {
                    yield return transition;
                }

                if (StateGroup != null)
                {
                    foreach (var transition in StateGroup.AllTransitionsEnumerable)
                    {
                        yield return transition;
                    }
                }
            }
        }

        /// <summary>
        /// 递归获取当前 state 的所有所属的 stateGroup
        /// </summary>
        public IEnumerable<StateBase> RecursionStateGroups
        {
            get
            {
                var group = StateGroup;
                while (group != null)
                {
                    yield return StateGroup;
                    group = group.StateGroup;
                }
            }
        }

        public event System.Action OnStateEnter;
        public event System.Action OnStateExit;
        public event System.Action OnStateUpdate;

        public StateBase(TiyaStateMachine stateMachine, StateBase stateGroup = null)
        {
            StateMachine = stateMachine;
            StateGroup = stateGroup;
        }

        /// <summary>
        /// 向 State 中添加一个转换
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(StateTransition transition) => _transitions.Add(transition);

        // 三个事件方法，其逻辑已经在 TiyaStateMachine 中处理了
        public void EnterState()
        {
            OnStateEnter?.Invoke();
        }
        public void ExitState()
        {
            OnStateExit?.Invoke();
        }
        public void UpdateState()
        {
            OnStateUpdate?.Invoke();
        }
    }

    /// <summary>
    /// StateBase 的泛型形式。参见 <seealso cref="StateBase"/>。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StateBase<T> : StateBase 
        where T : class
    {
        public new StateBase<T> StateGroup => base.StateGroup as StateBase<T>;
        public new T Subject => StateMachine.Subject as T;

        public new IEnumerable<StateBase<T>> RecursionStateGroups =>
            base.RecursionStateGroups.OfType<StateBase<T>>();

        public StateBase(TiyaStateMachine stateMachine, StateBase<T> stateGroup = null) : base(stateMachine, stateGroup) { }
    }
}
