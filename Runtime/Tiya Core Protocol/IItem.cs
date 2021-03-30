using UnityEngine;
using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 物品接口
    /// </summary>
    public interface IItem : IGameProperties
    {
        /// <summary>
        /// 物品种类 ID
        /// </summary>
        int ItemID { get; }

        string ItemName { get; }
        string Description { get; }

        /// <summary>
        /// 是否可堆叠。可堆叠物品同 ID 则是完全相同的，不可堆叠物品则是种类相同，但具体属性值等可能不同。
        /// 具体可见 <see cref="TiyaTools.ItemEquals(IItem, IItem)"/> 方法。
        /// </summary>
        bool IsStackable { get; }

        /// <summary>
        /// 物品种类
        /// </summary>
        TiyaItemType ItemType { get; }

        event System.Action<IActorController> OnUsingItem;

        /// <summary>
        /// 使用 item
        /// </summary>
        /// <param name="itemOwner"></param>
        void UseItem(IActorController itemOwner);
    }

    public enum TiyaItemType
    {
        /// <summary>
        /// 代表普通物品
        /// </summary>
        Material,
        /// <summary>
        /// 代表可以使用和取消使用的物品
        /// </summary>
        Equipment,
        /// <summary>
        /// 代表可以使用，使用后会消失的物品
        /// </summary>
        Expendable
    }

    public static partial class TiyaTools
    {
        /// <summary>
        /// 比较两个 item 是否时同一个 item
        /// </summary>
        /// <param name="lp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        public static bool ItemEquals(this IItem lp, IItem rp) =>
            lp.IsStackable ? lp.ItemID == rp.ItemID : lp.Equals(rp);

        /// <summary>
        /// 消耗一个存在于 inventory 中的 item。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="inventory"></param>
        public static void ConsumeItem(this IItem item, IInventory inventory)
        {
            inventory.ChangeInventoryItem(item, -1);
        }

        /// <summary>
        /// 装备一个武器 item
        /// </summary>
        /// <param name="item">武器 item</param>
        /// <param name="actor"></param>
        /// <param name="weaponPrefab">武器 prefab</param>
        public static void EquipWeapon(this IItem item, IActorController actor, GameObject weaponPrefab)
        {
            if (item.ItemType == TiyaItemType.Equipment)
            {
                var currentWeaponItem = actor.EquipmentManager.CurrentWeaponItem;
                if (currentWeaponItem == null || !currentWeaponItem.ItemEquals(item))
                {
                    actor.EquipmentManager.EquipWeapon(item, weaponPrefab);
                }
                else
                {
                    actor.EquipmentManager.EquipWeapon();
                }
            }
            else
            {
                Debug.LogError($"Item {item.ItemName} is not Equipment!");
            }
        }

        /// <summary>
        /// 装备一个武器 item
        /// </summary>
        /// <param name="item">武器 item</param>
        /// <param name="actor"></param>
        /// <param name="weaponPrefabPropertyName">武器 prefab 在 item 中的属性名称</param>
        public static void EquipWeapon(this IItem item, IActorController actor, string weaponPrefabPropertyName) =>
            item.EquipWeapon(actor, item.GetProperty<GameObject>(weaponPrefabPropertyName));
    }

}
