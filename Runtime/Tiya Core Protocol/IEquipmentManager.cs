using UnityEngine;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 游戏对象的装备物品的管理器
    /// </summary>
    public interface IEquipmentManager
    {
        // ---- Weapon ----

        /// <summary>
        /// 当前武器 Item
        /// </summary>
        IItem CurrentWeaponItem { get; }

        /// <summary>
        /// 当前武器实体
        /// </summary>
        IWeaponController CurrentWeapon { get; }
        
        /// <summary>
        /// 当前武器是否启用双手（左手） IK
        /// </summary>
        bool EnableTwoHandIK { get; set; }

        /// <summary>
        /// 装备武器
        /// </summary>
        /// <param name="weaponItem"></param>
        /// <param name="weaponPrefab"></param>
        void EquipWeapon(IItem weaponItem, GameObject weaponPrefab);

        /// <summary>
        /// 装备默认武器（卸下当前武器）
        /// </summary>
        void EquipWeapon();

        // TODO: ---- Other Equipments ----
        // Dictionary<string, IItem> ActorEquipments { get; }
    }
}
