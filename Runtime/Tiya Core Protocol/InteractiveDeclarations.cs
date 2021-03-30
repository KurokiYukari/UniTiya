using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 可交互接口
    /// </summary>
    public interface IInteractive
    {
        /// <summary>
        /// 可交互的 label，一般用于 GUI 显示。
        /// </summary>
        string InteractLabel { get; }

        GameObject InteractiveObject { get; }

        event System.Action<IActorController> OnInteracting;

        /// <summary>
        /// 执行交互逻辑
        /// </summary>
        /// <param name="actor"></param>
        void Interact(IActorController actor);
    }

    /// <summary>
    /// 交互主体接口
    /// </summary>
    public interface IInteractiveObjectHandler
    {
        /// <summary>
        /// 交互主体目前所有可交互的 IInteractive 对象
        /// </summary>
        ICollection<IInteractive> Interactives { get; }
    }
}
