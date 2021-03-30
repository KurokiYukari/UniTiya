using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.ItemSystem.Listeners
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Listener/Item Equip Weapon Listener")]
    public class ItemEquipWeaponListener : ScriptableObject
    {
        [SerializeField] string _weaponPrefabPropertyName = "WeaponPrefab";

        public void EquipItem(IItem item, IActorController actor) =>
            item.EquipWeapon(actor, _weaponPrefabPropertyName);
    }
}
