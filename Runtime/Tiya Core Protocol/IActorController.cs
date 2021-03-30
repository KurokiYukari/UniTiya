using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.Commands;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// Actor 模块的接口，定义了 Actor 的属性和无条件行为
    /// </summary>
    public interface IActorController
    {
        // ---- References ----

        GameObject GameObject { get; }

        /// <summary>
        /// 命令处理器
        /// </summary>
        ICommandProcessor<IActorController> CommandProcessor { get; }

        /// <summary>
        /// Actor 的游戏属性
        /// </summary>
        IActorProperties GameProperties { get; }

        /// <summary>
        /// Actor 的视角控制器
        /// </summary>
        IActorView ActorView { get; }

        /// <summary>
        /// Actor 的装备管理器
        /// </summary>
        IEquipmentManager EquipmentManager { get; }

        /// <summary>
        /// Actor 的背包
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// 用于获取 Actor 的世界定位和定向的 Transform。
        /// 建议不要直接用该 Transform 直接改变 actor 的位置和旋转（Tiya 语义上不保证这样的更改会生效），而是用 IActorController 提供的方法执行需要的操作
        /// </summary>
        Transform ActorTransform { get; }

        /// <summary>
        /// Actor 的 Animator
        /// </summary>
        Animator Animator { get; }

        /// <summary>
        /// Actor 的默认无条件行为
        /// </summary>
        IActorActions DefaultActions { get; }

        /// <summary>
        /// Actor 的行为重载
        /// </summary>
        IActorActions ActorActions { get; }

        // ---- Properties ----

        /// <summary>
        /// 指示是否是玩家
        /// </summary>
        bool IsPlayer { get; }

        /// <summary>
        /// 当前的速度
        /// </summary>
        Vector3 Velocity { get; }

        /// <summary>
        /// Actor 当前的移动方式，详见 <see cref="ActorLocomotionMode"/>
        /// </summary>
        ActorLocomotionMode LocomotionMode { get; set; }

        /// <summary>
        /// 被缩放到固定的范围的速度，一般用于传递给 Animator 设置动画。
        /// <para>
        /// 对应于 <see cref="LocomotionMode"/> ，scaledSpeed 的最大值被限定为：
        /// Walk -> 1 ; Run -> 2 ; Sprint -> 3
        /// </para>
        /// </summary>
        float ScaledSpeed { get; }

        /// <summary>
        /// Actor 与 AimPose 向关联的模式，详见 <see cref="ActorAimMode"/>
        /// </summary>
        ActorAimMode AimMode { get; set; }

        /// <summary>
        /// 是否在地面
        /// </summary>
        bool IsGround { get; }

        /// <summary>
        /// 是否在主动移动。被动的移动（如被击飞、一些动画的 rootMotion 等）都不会使其为 true。
        /// 在空中的状态（比如跳跃）会将当前的 IsMoving 状态锁定，直到 IsGround 为 true 才会改变。
        /// </summary>
        bool IsMoving { get; }

        /// <summary>
        /// 设置能否主动移动。将之设为 false 会强制停止当前的主动移动。
        /// </summary>
        bool CanMove { get; set; }

        /// <summary>
        /// 设置能否跳跃。
        /// </summary>
        bool CanJump { get; set; }

        // ---- Events ----

        /// <summary>
        /// 开始主动移动事件
        /// </summary>
        event System.Action OnStartMoving;

        /// <summary>
        /// 停止主动移动事件
        /// </summary>
        event System.Action OnStopMoving;

        /// <summary>
        /// 跳跃事件
        /// </summary>
        event System.Action OnJump;

        /// <summary>
        /// 离开地面事件
        /// </summary>
        event System.Action OnLeavingGround;

        /// <summary>
        /// 落地事件
        /// </summary>
        event System.Action OnLanding;

        /// <summary>
        /// 改变 <see cref="LocomotionMode"/> 事件，他在模式改变后的时候触发。
        /// <para>
        /// 第一个参数为前一个 LocomotionMode，第二个参数为当前 LocomotionMode
        /// </para>
        /// </summary>
        event System.Action<ActorLocomotionMode, ActorLocomotionMode> OnChangeLocomotionMode;

        // ---- Methods ----

        ///// <summary>
        ///// 强制位移，这个方法不属于主动移动。
        ///// </summary>
        ///// <param name="displacement">对象直立空间坐标</param>
        //void SimpleMove(Vector3 displacement);

        ///// <summary>
        ///// 先转向至世界空间的 direction 方向，然后持续向前移动
        ///// 所以输入 Vector3.zero 停止移动
        ///// </summary>
        ///// <param name="direction">其 y 方向会被忽略</param>
        //void TurnThenMoveForward(Vector3 direction);

        ///// <summary>
        ///// 保持自身 forward 不变，向世界空间的 direction 方向移动
        ///// </summary>
        ///// <param name="direction"></param>
        //void StrafeMove(Vector3 direction);

        ///// <summary>
        ///// 停止主动移动
        ///// </summary>
        //void StopMoving();

        ///// <summary>
        ///// 使自身转向 direction 方向（不会忽略 direction 的 y 值）
        ///// </summary>
        ///// <param name="direction"></param>
        //void TurnTo(Vector3 direction);

        ///// <summary>
        ///// 跳
        ///// </summary>
        //void Jump();
    }

    public interface IActorActions
    {
        // ---- Actor Actions ----

        /// <summary>
        /// 设置一个方向，然后尝试朝目标方向进行主动移动。
        /// 主动移动不保证一定会生效（比如在空中时，会设置方向但并不会立即朝目标方向移动，而是落地后再朝目标方向移动）
        /// </summary>
        /// <param name="direction"></param>
        void Move(Vector3 direction);
        void SimpleMove(Vector3 displacement);
        void SwitchLocomotionMode(ActorLocomotionMode mode);
        void Jump();

        // ---- View Actions ----

        void View(Vector2 direction);
        void Lock(LockCmdType cmdType);
    }

    public enum LockCmdType
    {
        Lock, Unlock, LockToNextTarget, LockToPreTarget
    }

    /// <summary>
    /// Actor 与其 <see cref="IActorController.AimTransform"/> 关联的方式。
    /// </summary>
    public enum ActorAimMode
    {
        /// <summary>
        /// 默认关联模式，不做改变。
        /// </summary>
        LookAtActorForward,
        /// <summary>
        /// 看向 AimPos。详见 <see cref="TiyaTools.GetAimFocusPosition(IActorController)"/>。
        /// </summary>
        LookAtAimPos,
        /// <summary>
        /// 转动上半身，使之朝向 actor 的 <see cref="IActorView.ViewTransform"/> forward 方向。
        /// </summary>
        TurnUpperBodyToAimDirection,
    }

    /// <summary>
    /// Actor 的移动模式
    /// </summary>
    public enum ActorLocomotionMode
    {
        /// <summary>
        /// 速度更慢的 Run
        /// </summary>
        Walk,
        /// <summary>
        /// 一般作为默认模式使用
        /// </summary>
        Run,
        /// <summary>
        /// Sprint 会强制将人物转向移动的方向
        /// </summary>
        Sprint,
    }

    public static partial class TiyaTools
    {
        /// <summary>
        /// 获取 actor 瞄准的焦点。
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Vector3 GetAimFocusPosition(this IActorController actor)
        {
            // TODO: 过滤射线检测的 Collider
            var viewTransform = actor.ActorView.ViewTransform;
            if (Physics.Raycast(viewTransform.position, viewTransform.forward, out RaycastHit hit, Camera.main.farClipPlane))
            {
                return hit.point;
            }
            else
            {
                return viewTransform.position + viewTransform.forward * Camera.main.farClipPlane;
            }
        }
    }
}
