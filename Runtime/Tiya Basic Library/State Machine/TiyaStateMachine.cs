using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.StateMachine
{
    /// <summary>
    /// 状态机基类，这个基类已经处理好了状态的转换与更新。
    /// 对这个状态机的配置需要通过继承，在 Awake 中配置状态机的所有状态。
    /// </summary>
    public abstract class TiyaStateMachine : MonoBehaviour
    {
        [Header("Debug")]
        [TiyaPropertyAttributes.Disable]
        [SerializeField] internal string _currentStateName;

        public abstract object Subject { get; }

        public abstract string EntryStateId { get; }
        public StateBase State { get; set; }

        private readonly Dictionary<string, StateBase> _stateDictionary = new Dictionary<string, StateBase>();
        public StateBase this[string stateId]
        {
            get => _stateDictionary[stateId];
            set => _stateDictionary[stateId] = value;
        }

        protected void Start()
        {
            State = _stateDictionary[EntryStateId];
            State.EnterState();
        }

        protected void Update()
        {
            StateMachineUpdate();
        }

        private void StateMachineUpdate()
        {
            foreach (var transition in State.AllTransitionsEnumerable)
            {
                if (transition.CheckTransition())
                {
                    State.ExitState();

                    StateBase destinationState;
                    if (transition.DestinationStateId == null)
                    {
                        destinationState = _stateDictionary[EntryStateId];
                    }
                    else
                    {
                        destinationState = _stateDictionary[transition.DestinationStateId];
                    }

                    // 计算 StateGroup 的 OnStateExit 和 OnStateEnter
                    // 感觉这样算效率可能有问题？所以尽量不要使用 stateGroup = =
                    var preGroups = State.RecursionStateGroups;
                    var destinationStateGroups = destinationState.RecursionStateGroups.ToList();
                    if (preGroups.Any())
                    {
                        foreach (var group in preGroups)
                        {
                            var index = destinationStateGroups.IndexOf(group);
                            if (index < 0) // group 在目标状态的 groups 中不存在，则 Exit
                            {
                                group.ExitState();
                            }
                            else // group 在目标状态中存在，将其在目标状态之后的 group Enter
                            {
                                for (int i = index - 1; i >= 0; i--)
                                {
                                    destinationStateGroups[i].EnterState();
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = destinationStateGroups.Count - 1; i >= 0; i--)
                        {
                            destinationStateGroups[i].EnterState();
                        }
                    }

                    State = destinationState;

                    State.EnterState();
                    transition.DoTransition();

                    break;
                }
            }
            _currentStateName = State.GetType().Name;

            // 执行 State 的 OnStateUpdate
            State.UpdateState();
            // 执行 State 的 Group 的 OnStateUpdate
            foreach (var group in State.RecursionStateGroups)
            {
                group.UpdateState();
            }
        }

        public bool IsInState(string stateId)
        {
            return this[stateId] == State;
        }
    }
}
