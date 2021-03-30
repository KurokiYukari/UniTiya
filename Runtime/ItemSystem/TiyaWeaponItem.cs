using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.ItemSystem
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Tiya Weapon Item")]
    public class TiyaWeaponItem : TiyaItem
    {
        [TypeRestriction(typeof(IWeaponController))]
        [SerializeField] GameObject _weaponPrefab;

        public GameObject WeaponPrefab => _weaponPrefab;

        protected override void UseItemOverride(IActorController itemOwner)
        {
            this.EquipWeapon(itemOwner, WeaponPrefab);
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            try
            {
                _weaponPrefab = GetProperty<GameObject>(nameof(WeaponPrefab));
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            SetProperty("Weapon", true, true);
            SetProperty(nameof(WeaponPrefab), _weaponPrefab, true);
        }
    }
}
