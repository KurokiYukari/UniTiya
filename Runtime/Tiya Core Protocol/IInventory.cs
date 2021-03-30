using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 背包系统接口
    /// </summary>
    public interface IInventory
    {
        event System.Action<IItem, int> OnChangeInventoryItem;

        /// <summary>
        /// 向背包中添加或删除指定 item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemCount"></param>
        void ChangeInventoryItem(IItem item, int itemCount);

        /// <summary>
        /// 获取当前背包中 item 的数量
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetItemCount(IItem item);

        /// <summary>
        /// 遗弃背包中的 item，并在指定 position 生成拾取该物体的可交互 GameObject
        /// </summary>
        /// <param name="items"></param>
        /// <param name="position"></param>
        void AbandonItem(IEnumerable<(IItem item, int itemCount)> items, Vector3? position = null);
    }
}
