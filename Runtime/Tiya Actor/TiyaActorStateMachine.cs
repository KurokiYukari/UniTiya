using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.StateMachine;

namespace Sarachan.UniTiya.TiyaActor
{
    /// <summary>
    /// 适用于所有 IActorController 的状态机
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/StateMachine/Tiya Actor StateMachine")]
    public class TiyaActorStateMachine : TiyaStateMachine, IActorActions
    {
        IActorController _subject;
        public override object Subject => _subject ??= GetComponent<IActorController>();
        public override string EntryStateId => IDLE_STATE;

        new ActorState State => base.State as ActorState;

        new ActorState this[string stateId]
        {
            //get => base[stateId] as ActorState;
            set => base[stateId] = value;
        }

        // 向 State 的委托
        public void Jump() => State.Jump();
        public void Lock(LockCmdType cmdType) => State.Lock(cmdType);
        public void Move(Vector3 direction) => State.Move(direction);
        public void SimpleMove(Vector3 displacement) => State.SimpleMove(displacement);
        public void SwitchLocomotionMode(ActorLocomotionMode mode) => State.SwitchLocomotionMode(mode);
        public void View(Vector2 direction) => State.View(direction);

        protected void Awake()
        {
            InitStates();
        }

        #region Avaliable States
        // Groups
        //public const string FREEGROUND_STATEGROUP = "FREEGROUND_GROUP";

        public const string IDLE_STATE = "IDLE_STATE";
        public const string LOCOMOTION_STATE = "LOCOMOTION_STATE";
        public const string FREEFALL_STATE = "FREEFALL_STATE";

        public const string DEAD_STATE = "DEAD_STATE";

        void InitStates()
        {
            //this[FREEGROUND_STATEGROUP] = new ActorFreeGroundStateGroup(this);

            this[IDLE_STATE] = new ActorIdleState(this);
            this[LOCOMOTION_STATE] = new ActorLocomotionState(this);
            this[FREEFALL_STATE] = new ActorFreeFallState(this);

            this[DEAD_STATE] = new ActorDeadState(this);
        }
        #endregion

        #region States Implementation
        abstract class ActorState : StateBase<IActorController>, IActorActions
        {
            public new TiyaActorStateMachine StateMachine => base.StateMachine as TiyaActorStateMachine;
            public new ActorState StateGroup => base.StateGroup as ActorState;

            public ActorState(TiyaStateMachine stateMachine, StateBase<IActorController> stateGroup = null) : base(stateMachine, stateGroup)
            {
            }

            public event System.Action OnJump;
            public event System.Action<LockCmdType> OnLock;
            public event System.Action<Vector3> OnMove;
            public event System.Action<Vector3> OnSimpleMove;
            public event System.Action<ActorLocomotionMode> OnSwitchLocomotionMode;
            public event System.Action<Vector2> OnView;

            public void Jump()
            {
                StateGroup?.Jump();
                OnJump?.Invoke();
            }
            public void Lock(LockCmdType cmdType)
            {
                StateGroup?.Lock(cmdType);
                OnLock?.Invoke(cmdType);

                Subject.DefaultActions.Lock(cmdType);
            }
            public void Move(Vector3 direction)
            {
                StateGroup?.Move(direction);
                OnMove?.Invoke(direction);

                Subject.DefaultActions.Move(direction);
            }
            public void SimpleMove(Vector3 displacement)
            {
                StateGroup?.SimpleMove(displacement);
                OnSimpleMove?.Invoke(displacement);

                Subject.DefaultActions.SimpleMove(displacement);
            }
            public void SwitchLocomotionMode(ActorLocomotionMode mode)
            {
                StateGroup?.SwitchLocomotionMode(mode);
                OnSwitchLocomotionMode?.Invoke(mode);

                Subject.DefaultActions.SwitchLocomotionMode(mode);
            }
            public void View(Vector2 direction)
            {
                StateGroup?.View(direction);
                OnView?.Invoke(direction);

                Subject.DefaultActions.View(direction);
            }
        }

        abstract class ActorAliveStateBase : ActorState
        {
            public ActorAliveStateBase(TiyaStateMachine stateMachine, StateBase<IActorController> stateGroup = null) : base(stateMachine, stateGroup)
            {
                // Transition: AnyState -> DeadState
                var toDeadTransition = new StateTransition(DEAD_STATE, () => !Subject.IsAlive);
            }
        }

        class ActorDeadState : ActorState
        {
            public ActorDeadState(TiyaStateMachine stateMachine) : base(stateMachine)
            {
                // Transition -> Default
                var exitTransition = new StateTransition(triggerCondition: () => Subject.IsAlive);
                AddTransition(exitTransition);
            }
        }

        class ActorFreeGroundStateGroup : ActorAliveStateBase
        {
            public ActorFreeGroundStateGroup(TiyaStateMachine stateMachine) : base(stateMachine)
            {
                // Transition -> FreeFall
                var toFreeFallTransition = new StateTransition(
                    FREEFALL_STATE,
                    () => !Subject.IsGround);
                AddTransition(toFreeFallTransition);

                OnJump += Subject.DefaultActions.Jump;
            }
        }

        /// <summary>
        /// Idle 状态，代表站立闲置状态。
        /// 是状态机的初始状态。
        /// </summary>
        class ActorIdleState : ActorFreeGroundStateGroup
        {
            public ActorIdleState(TiyaActorStateMachine stateMachine) : base(stateMachine)
            {
                // Transition -> Locomotion
                var toLocomotionTransition = new StateTransition(
                    LOCOMOTION_STATE,
                    () => Subject.IsMoving);
                AddTransition(toLocomotionTransition);
            }
        }

        /// <summary>
        /// Locomotion 状态，代表移动状态。属于状态组 <see cref="PlayerFreeGroundStateGroup"/>。
        /// </summary>
        class ActorLocomotionState : ActorFreeGroundStateGroup
        {
            public ActorLocomotionState(TiyaActorStateMachine stateMachine) : base(stateMachine)
            {
                // Transition : Locomotion -> Idle
                var toIdleTransition = new StateTransition(
                    IDLE_STATE,
                    () => !Subject.IsMoving);
                AddTransition(toIdleTransition);
            }
        }

        /// <summary>
        /// FreeFall 状态
        /// </summary>
        class ActorFreeFallState : ActorAliveStateBase
        {
            public ActorFreeFallState(TiyaActorStateMachine stateMachine) : base(stateMachine)
            {
                // Transition -> Idle
                var toIdleTransition = new StateTransition(
                    IDLE_STATE,
                    () => Subject.IsGround);
                AddTransition(toIdleTransition);
            }
        }
        #endregion
    }
}