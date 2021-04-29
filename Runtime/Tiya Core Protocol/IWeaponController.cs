using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.Skill;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// WeaponController 接口
    /// </summary>
    public interface IWeaponController
    {
        GameObject WeaponGameObject { get; }

        /// <summary>
        /// Weapon 的 Owener。
        /// 大多数情况下这个属性时动态的，注意要在 <see cref="IEquipmentManager"/> 装备
        /// Weapon 时设置 Owner。
        /// 也因为这个原因，很多根据 Owner 的初始化设定需要通过该属性的 setter 实现。
        /// </summary>
        IActorController Owner { get; set; }

        ISkill NormalSkill { get; }
        ISkill SpecialSkill { get; }
        IReadOnlyList<ISkill> ExtraSkills { get; }

        /// <summary>
        /// Weapon 行为的默认无条件行为
        /// </summary>
        IWeaponActorActions DefaultActions { get; }

        /// <summary>
        /// Weapon 的具体行为
        /// </summary>
        IWeaponActorActions ActorWeaponActions { get; }

        /// <summary>
        /// 当该 Weapon 被装备给 Actor 时的时间。一般在 <see cref="Owner"/> setter 执行时触发。
        /// </summary>
        event System.Action<IActorController> OnEquip;
    }

    /// <summary>
    /// WeaponController 提供给 Actor 的方法接口。
    /// UniTiya 中默认提供两个攻击入口 <see cref="NormalAttack(SkillCmdType)"/> <see cref="SpecialAttack(SkillCmdType)"/>
    /// </summary>
    public interface IWeaponActorActions
    {
        /// <summary>
        /// 普通攻击
        /// </summary>
        /// <param name="type"></param>
        void NormalAttack(SkillCmdType type);
        /// <summary>
        /// 特殊攻击
        /// </summary>
        /// <param name="type"></param>
        void SpecialAttack(SkillCmdType type);
        /// <summary>
        /// 额外攻击入口
        /// </summary>
        IList<System.Action<SkillCmdType>> ExtraSkillTriggers { get; }
    }

    /// <summary>
    /// 攻击的模式。
    /// 一般来说，瞬时攻击只会使用到 PerformSkill，而另外一些持续性的攻击，则还需要 CancelSkill
    /// 来取消持续攻击。
    /// </summary>
    public enum SkillCmdType
    {
        PerformSkill,
        CancelSkill
    }
}
