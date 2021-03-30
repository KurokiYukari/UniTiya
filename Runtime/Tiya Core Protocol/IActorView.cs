using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// Actor 的视线控制接口
    /// </summary>
    public interface IActorView
    {
        /// <summary>
        /// Actor View 的 Transform
        /// 对于 Player，ViewTransform 的 forward 一般是与 Camera forward 相同的
        /// </summary>
        Transform ViewTransform { get; }

        /// <summary>
        /// 是否锁定敌人
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// 锁定的对象。
        /// </summary>
        GameObject LockTarget { get; }

        /// <summary>
        /// 根据 direction 指⽰的⽅向改变当前视⾓
        /// </summary>
        /// <param name="deltaRotation"></param>
        void View(Vector2 deltaRotation);

        /// <summary>
        /// 将 View 还原到初始状态
        /// </summary>
        void ResetView();

        /// <summary>
        /// 将视角锁定向附近的敌人。
        /// </summary>
        /// <param name="lockCmdType"></param>
        void Lock(LockCmdType lockCmdType);

        // TODO: 完善这个方法
        /// <summary>
        /// 制造一个暂时的强制视角偏移。
        /// </summary>
        /// <param name="pitchAngle"></param>
        void TempRotate(float pitchAngle);

        event System.Action<GameObject> OnLock;
        event System.Action<GameObject> OnUnlock;
    }
}
