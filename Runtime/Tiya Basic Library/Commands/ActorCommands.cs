using System;
using UnityEngine;

namespace Sarachan.UniTiya.Commands
{
    public static class ActorCommands
    {
        // ---- Actor Commands ----
        public static ICommand<IActorController> Move(Vector3 direction) => new MoveCommand(direction);
        public static ICommand<IActorController> SimpleMove(Vector3 displacement) => new SimpleMoveCommand(displacement);
        public static ICommand<IActorController> SwitchLocomotionMode(ActorLocomotionMode mode) => new SwitchLocomotionModeCommand(mode);
        public static ICommand<IActorController> Jump() => new JumpCommand();
        public static ICommand<IActorController> SwitchAimMode(ActorAimMode mode) => new SwitchAimModeCommand(mode);

        // ---- View Commands ----
        public static ICommand<IActorController> View(Vector2 deltaRotation) => new ViewCommand(deltaRotation);
        public static ICommand<IActorController> Lock(LockCmdType lockCmdType) => new LockCommand(lockCmdType);
        public static ICommand<IActorController> Lock(GameObject lockTarget) => new LockToCommand(lockTarget);

        // ---- Equipment Commands ----
        public static ICommand<IActorController> EquipDefaultWeapon() => new EquipDefaultWeaponCommand();
        public static ICommand<IActorController> NormalAttack(SkillCmdType skillCmdType) => new NormalAttackCommand(skillCmdType);
        public static ICommand<IActorController> SpecialAttack(SkillCmdType skillCmdType) => new SpecialAttackCommand(skillCmdType);
        public static ICommand<IActorController> WeaponExtraSkill(int extraSkillIndex, SkillCmdType skillCmdType) => new WeaponExtraSkillCommand(extraSkillIndex, skillCmdType);

        // ---- Other Commands ----
        public static ICommand<IActorController> UseItem(IItem item) => new UseItemCommand(item); 

        #region Command Implementations
        readonly struct MoveCommand : ICommand<IActorController>
        {
            public Vector3 Direction { get; }

            public MoveCommand(Vector3 direction)
            {
                Direction = direction;
            }

            public void Execute(IActorController subject) => subject.ActorActions.Move(Direction);
        }

        readonly struct SimpleMoveCommand : ICommand<IActorController>
        {
            public Vector3 Displacement { get; }

            public SimpleMoveCommand(Vector3 displacement)
            {
                Displacement = displacement;
            }

            public void Execute(IActorController subject) => subject.ActorActions.SimpleMove(Displacement);
        }

        readonly struct JumpCommand : ICommand<IActorController>
        {
            public void Execute(IActorController subject) => subject.ActorActions.Jump();
        }

        readonly struct SwitchLocomotionModeCommand : ICommand<IActorController>
        {
            public ActorLocomotionMode LocomotionMode { get; }

            public SwitchLocomotionModeCommand(ActorLocomotionMode locomotionMode)
            {
                LocomotionMode = locomotionMode;
            }

            public void Execute(IActorController subject) => subject.ActorActions.SwitchLocomotionMode(LocomotionMode);
        }

        readonly struct ViewCommand : ICommand<IActorController>
        {
            public Vector2 DeltaRotation { get; }

            public ViewCommand(Vector2 deltaRotation)
            {
                DeltaRotation = deltaRotation;
            }

            public void Execute(IActorController subject) => subject.ActorActions.View(DeltaRotation);
        }

        readonly struct LockCommand : ICommand<IActorController>
        {
            public LockCmdType LockCmdType { get; }

            public LockCommand(LockCmdType lockCmdType)
            {
                LockCmdType = lockCmdType;
            }

            public void Execute(IActorController subject) => subject.ActorActions.Lock(LockCmdType);
        }

        readonly struct LockToCommand : ICommand<IActorController>
        {
            public GameObject LockTarget { get; }

            public LockToCommand(GameObject lockTarget)
            {
                LockTarget = lockTarget ?? throw new ArgumentNullException(nameof(lockTarget));
            }

            public void Execute(IActorController subject) => subject.ActorView.LockTo(LockTarget);
        }

        // TODO: 是否要对 WeaponController 使用命令模式转接？
        readonly struct NormalAttackCommand : ICommand<IActorController>
        {
            public SkillCmdType AttackCmdType { get; }

            public NormalAttackCommand(SkillCmdType type)
            {
                AttackCmdType = type;
            }

            public void Execute(IActorController subject)
            {
                if (subject.EquipmentManager != null)
                {
                    subject.EquipmentManager.CurrentWeapon.ActorWeaponActions.NormalAttack(AttackCmdType);
                }
            }
        }

        readonly struct SpecialAttackCommand : ICommand<IActorController>
        {
            public SkillCmdType AttackCmdType { get; }

            public SpecialAttackCommand(SkillCmdType type)
            {
                AttackCmdType = type;
            }

            public void Execute(IActorController subject)
            {
                if (subject.EquipmentManager != null)
                {
                    subject.EquipmentManager.CurrentWeapon.ActorWeaponActions.SpecialAttack(AttackCmdType);
                }
            }
        }

        readonly struct WeaponExtraSkillCommand : ICommand<IActorController>
        {
            public int ExtraSkillIndex { get; }
            public SkillCmdType AttackCmdType { get; }

            public WeaponExtraSkillCommand(int index, SkillCmdType type)
            {
                ExtraSkillIndex = index;
                AttackCmdType = type;
            }

            public void Execute(IActorController subject)
            {
                if (subject.EquipmentManager != null)
                {
                    subject.EquipmentManager.CurrentWeapon.ActorWeaponActions.ExtraSkillTriggers[ExtraSkillIndex](AttackCmdType);
                }
            }
        }

        readonly struct SwitchAimModeCommand : ICommand<IActorController>
        {
            public ActorAimMode AimMode { get; }

            public SwitchAimModeCommand(ActorAimMode aimMode)
            {
                AimMode = aimMode;
            }

            public void Execute(IActorController subject)
            {
                subject.AimMode = AimMode;
            }
        }

        readonly struct UseItemCommand : ICommand<IActorController>
        {
            public IItem Item { get; }

            public UseItemCommand(IItem item)
            {
                Item = item;
            }

            public void Execute(IActorController subject)
            {
                Item.UseItem(subject);
            }
        }

        readonly struct EquipDefaultWeaponCommand : ICommand<IActorController>
        {
            public void Execute(IActorController subject)
            {
                subject.EquipmentManager.EquipWeapon();
            }
        }
        #endregion
    }
}
