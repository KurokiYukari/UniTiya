using System;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

using Sarachan.UniTiya.StateMachine;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaWeapon
{
    /// <summary>
    /// 适用于 <see cref="IWeaponController"/> 的可配置状态机。应该与之放在同一个 GameObject 中。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/StateMachine/Tiya Weapon StateMachine")]
    public class TiyaWeaponStateMachine : TiyaStateMachine, IWeaponActorActions
    {
        [Serializable]
        struct AttackConfiguration
        {
            [Tooltip("If enable, attack will be auto canceled after a while.")]
            [SerializeField] bool _autoCancelAttack;

            [Tooltip("Auto cancel duration after attack performing.")]
            [SerializeField] [HideIf(nameof(_autoCancelAttack), false)] float _attackingDuration;

            [SerializeField] bool _canMoveWhileAttacking;
            [HideIf(nameof(_canMoveWhileAttacking), true)]
            [SerializeField] float _stopMoveOffset;

            [SerializeField] bool _canAttackWhileNotInGround;

            [SerializeField] bool _rootMotion;

            public bool AutoCancelAttack => _autoCancelAttack;
            public float AttackingDuration => _attackingDuration;
            public bool CanMoveWhileAttacking => _canMoveWhileAttacking;
            public float StopMoveOffset => _stopMoveOffset;
            public bool CanAttackWhileNotInGround => _canAttackWhileNotInGround;
            public bool RootMotion => _rootMotion;

            public static AttackConfiguration DefaultConfiguration => new AttackConfiguration()
            {
                _autoCancelAttack = true,
            };
        }

        [SerializeField] AttackConfiguration _normalAttackConfiguration;
        [SerializeField] AttackConfiguration _specialAttackConfiguration;
        [SerializeField] AttackConfiguration[] _extraAttackConfigurations;

        IWeaponController _weapon;
        public IWeaponController WeaponSubject => _weapon ??= GetComponent<IWeaponController>();

        public override object Subject => WeaponSubject;
        public override string EntryStateId => FREE_STATE;

        new WeaponState State => base.State as WeaponState;

        public void NormalAttack(SkillCmdType type) => State.NormalAttack(type);
        public void SpecialAttack(SkillCmdType type) => State.SpecialAttack(type);
        public IList<Action<SkillCmdType>> ExtraSkillTriggers => State.ExtraSkillTriggers;

        protected void Awake()
        {
            if (_extraAttackConfigurations.Length > WeaponSubject.DefaultActions.ExtraSkillTriggers.Count)
            {
                Debug.LogWarning($"{nameof(_extraAttackConfigurations)}'s count is larger than count of weapon " +
                    $"{WeaponSubject.WeaponGameObject.name}'s extra skills. Overflowed configurations will be ignored.");
            }

            InitStates();
        }

        #region Avaliable States
        public const string FREE_STATE = "FREE_STATE";
        public const string NORMAL_ATTACKING_STATE = "NORMAL_ATTACKING_STATE";
        public const string SPECIAL_ATTACKING_STATE = "SPECIAL_ATTACKING_STATE";
        public const string EXTRA_ATTACKING_STATE = "EXTRA_ATTACKING_STATE";

        void InitStates()
        {
            this[FREE_STATE] = new FreeState(this);
            this[NORMAL_ATTACKING_STATE] = new AttackingState(this, AttackType.NormalAttack, _normalAttackConfiguration);
            this[SPECIAL_ATTACKING_STATE] = new AttackingState(this, AttackType.SpecialAttack, _specialAttackConfiguration);
            this[EXTRA_ATTACKING_STATE] = new AttackingState(this, AttackType.GetExtraSkillAttackType(0), new AttackConfiguration());
        }
        #endregion

        #region States Implementation
        abstract class WeaponState : StateBase<IWeaponController>, IWeaponActorActions
        {
            public new TiyaWeaponStateMachine StateMachine => base.StateMachine as TiyaWeaponStateMachine;

            public WeaponState(TiyaStateMachine stateMachine, StateBase<IWeaponController> stateGroup = null) : base(stateMachine, stateGroup)
            {
            }

            public abstract IList<Action<SkillCmdType>> ExtraSkillTriggers { get; }

            public abstract void NormalAttack(SkillCmdType type);
            public abstract void SpecialAttack(SkillCmdType type);
        }

        class FreeState : WeaponState
        {
            bool _toNormalAttackingState = false;
            bool _toSpecialAttackingState = false;
            bool _toExtraAttackingState = false;

            public FreeState(TiyaStateMachine stateMachine, StateBase<IWeaponController> stateGroup = null) : base(stateMachine, stateGroup)
            {
                var toNormalAttackingStateTransition = new StateTransition(NORMAL_ATTACKING_STATE, () => _toNormalAttackingState);
                AddTransition(toNormalAttackingStateTransition);

                var toSpecialAttackingStateTransition = new StateTransition(SPECIAL_ATTACKING_STATE, () => _toSpecialAttackingState);
                AddTransition(toSpecialAttackingStateTransition);

                var toExtraAttackingStateTransition = new StateTransition(EXTRA_ATTACKING_STATE, () => _toExtraAttackingState);
                AddTransition(toExtraAttackingStateTransition);

                OnStateEnter += OnStateEnterListener;
            }

            void OnStateEnterListener()
            {
                _toNormalAttackingState = false;
                _toSpecialAttackingState = false;
                _toExtraAttackingState = false;
            }

            List<Action<SkillCmdType>> _extraSkillTriggers;
            public override IList<Action<SkillCmdType>> ExtraSkillTriggers
            {
                get
                {
                    if (_extraSkillTriggers == null)
                    {
                        _extraSkillTriggers = new List<Action<SkillCmdType>>();
                        for (int i = 0; i < Subject.DefaultActions.ExtraSkillTriggers.Count; i++)
                        {
                            var defaultTrigger = Subject.DefaultActions.ExtraSkillTriggers[i];

                            AttackConfiguration conf;
                            try
                            {
                                conf = StateMachine._extraAttackConfigurations[i];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                conf = AttackConfiguration.DefaultConfiguration;
                            }

                            var localIndex = i;
                            _extraSkillTriggers.Add(ExtraSkillListener);

                            void ExtraSkillListener(SkillCmdType cmd)
                            {
                                if (cmd == SkillCmdType.PerformSkill)
                                {
                                    if (!conf.CanAttackWhileNotInGround && !Subject.Owner.IsGround)
                                    {
                                        return;
                                    }

                                    if (!(conf.AutoCancelAttack && conf.AttackingDuration <= 0))
                                    {
                                        var attackState = StateMachine[EXTRA_ATTACKING_STATE] as AttackingState;
                                        attackState.Conf = conf;
                                        attackState.AttackType = AttackType.GetExtraSkillAttackType(localIndex);
                                        _toExtraAttackingState = true;
                                    }
                                }

                                defaultTrigger(cmd);
                            }
                        }
                    }
                    return _extraSkillTriggers;
                }
            }

            public override void NormalAttack(SkillCmdType type)
            {
                if (type == SkillCmdType.PerformSkill)
                {
                    if (!StateMachine._normalAttackConfiguration.CanAttackWhileNotInGround && !Subject.Owner.IsGround)
                    {
                        return;
                    }

                    _toNormalAttackingState = true;
                }
                Subject.DefaultActions.NormalAttack(type);
            }

            public override void SpecialAttack(SkillCmdType type)
            {
                if (type == SkillCmdType.PerformSkill)
                {
                    if (!StateMachine._specialAttackConfiguration.CanAttackWhileNotInGround && !Subject.Owner.IsGround)
                    {
                        return;
                    }

                    _toSpecialAttackingState = true;
                }
                Subject.DefaultActions.SpecialAttack(type);
            }
        }

        class AttackingState : WeaponState
        {
            bool _attackFinished = false;

            bool _originCanMove;
            IDisposable _changeCanMoveSubscribe;
            bool _originCanJump;

            bool _originRootMotion;

            public AttackType AttackType { get; internal set; }

            public AttackConfiguration Conf { get; set; }

            public AttackingState(TiyaStateMachine stateMachine, AttackType attackType, AttackConfiguration conf, StateBase<IWeaponController> stateGroup = null) 
                : base(stateMachine, stateGroup)
            {
                AttackType = attackType;
                Conf = conf;

                var toFreeStateTransition = new StateTransition(FREE_STATE, () => _attackFinished);
                AddTransition(toFreeStateTransition);

                OnStateEnter += OnStateEnterListener;
                OnStateExit += OnStateExitListener;
            }

            void OnStateEnterListener()
            {
                _attackFinished = false;

                if (Conf.AutoCancelAttack)
                {
                    Observable.Timer(TimeSpan.FromSeconds(Conf.AttackingDuration))
                        .Subscribe(_ => _attackFinished = true);
                }

                _originCanMove = Subject.Owner.CanMove;
                _originCanJump = Subject.Owner.CanJump;

                _originRootMotion = Subject.Owner.ApplyRootMotion;
                
                if (!Conf.CanMoveWhileAttacking && Conf.StopMoveOffset > 0)
                {
                    _changeCanMoveSubscribe?.Dispose();
                    _changeCanMoveSubscribe = Observable.Timer(TimeSpan.FromSeconds(Conf.StopMoveOffset))
                        .Subscribe(_ =>
                        {
                            Subject.Owner.CanMove = Conf.CanMoveWhileAttacking;
                        });
                }
                else
                {
                    Subject.Owner.CanMove = Conf.CanMoveWhileAttacking;
                }

                Subject.Owner.CanJump = Conf.CanAttackWhileNotInGround;

                Subject.Owner.ApplyRootMotion = Conf.RootMotion;
            }

            void OnStateExitListener()
            {
                Subject.Owner.CanMove = _originCanMove;
                _changeCanMoveSubscribe?.Dispose();
                _changeCanMoveSubscribe = null;

                Subject.Owner.CanJump = _originCanJump;

                Subject.Owner.ApplyRootMotion = _originRootMotion;
            }

            List<Action<SkillCmdType>> _extraSKillTrigger;
            public override IList<Action<SkillCmdType>> ExtraSkillTriggers
            {
                get
                {
                    if (_extraSKillTrigger == null)
                    {
                        _extraSKillTrigger = new List<Action<SkillCmdType>>();

                        for (int i = 0; i < Subject.DefaultActions.ExtraSkillTriggers.Count; i++)
                        {
                            var localIndex = i;
                            _extraSKillTrigger.Add(cmd =>
                            {
                                if (localIndex == AttackType.ToExtraSkillIndex())
                                {
                                    StateCancelSkill(cmd, Subject.DefaultActions.ExtraSkillTriggers[localIndex]);
                                }
                            });
                        }
                    }
                    return _extraSKillTrigger;
                }
            }

            public override void NormalAttack(SkillCmdType type)
            {
                if (AttackType == AttackType.NormalAttack)
                {
                    StateCancelSkill(type, Subject.DefaultActions.NormalAttack);
                }
            }

            public override void SpecialAttack(SkillCmdType type)
            {
                if (AttackType == AttackType.SpecialAttack)
                {
                    StateCancelSkill(type, Subject.DefaultActions.SpecialAttack);
                }
            }

            void StateCancelSkill(SkillCmdType cmd, Action<SkillCmdType> defaultSkill)
            {
                if (cmd == SkillCmdType.CancelSkill)
                {
                    if (!Conf.AutoCancelAttack)
                    {
                        _attackFinished = true;
                    }
                    defaultSkill(cmd);
                }
            }
        }
        #endregion
    }
}
