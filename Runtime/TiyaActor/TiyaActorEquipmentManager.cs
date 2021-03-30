using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaActor
{
    /// <summary>
    /// 适用于 Actor 的 IEquipmentManager 实现
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Actor/Tiya Actor Equipment Manager")]
    public class TiyaActorEquipmentManager : MonoBehaviour, IEquipmentManager
    {
        [SerializeField] GameObject _defaultWeapon;

        [Header("Debug")]
        [SerializeField] GameObject _weaponObject;
        
        public IItem CurrentWeaponItem { get; private set; }
        public IWeaponController CurrentWeapon { get; private set; }

        IActorController _actorController;
        Animator _animator;

        Transform _rightHand;
        Transform _leftHand;

        public bool EnableTwoHandIK { get; set; } = true;

        #region Unity Events
        private void Awake()
        {
            _actorController = GetComponent<IActorController>();
            _animator = GetComponent<Animator>();

            _rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            _leftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
        }

        private void Start()
        {
            if (_defaultWeapon != null)
            {
                EquipWeapon();
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (CurrentWeapon == null)
            {
                return;
            }

            // 双持时左手 IK
            // FIXME: 不生效？
            //if (EnableTwoHandIK && _currentWeapon.LeftHandIKCtrl != null)
            //{
            //    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            //    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _currentWeapon.LeftHandIKCtrl.position);
            //}
        }
        #endregion

        public void EquipWeapon(IItem weaponItem, GameObject weaponPrefab)
        {
            CurrentWeaponItem = weaponItem;
            EquipWeaponPrefab(weaponPrefab);
        }

        public void EquipWeapon()
        {
            CurrentWeaponItem = null;
            EquipWeaponPrefab(_defaultWeapon);
        }

        void EquipWeaponPrefab(GameObject weaponPrefab)
        {
            // create weapon
            GameObject weaponObj = Instantiate(weaponPrefab, _rightHand);
            var weapon = weaponObj.GetComponent<IWeaponController>();

            Debug.Assert(weapon != null, $"{weaponPrefab.name} doesnt have {nameof(IWeaponController)} Component!");

            // init weapon
            weapon.Owner = _actorController;
            if (_actorController.IsPlayer)
            {
                foreach (var trans in weaponObj.GetComponentsInChildren<Transform>())
                {
                    trans.gameObject.layer = LayerMask.NameToLayer("Player Weapon");
                }
            }

            // place weapon
            var rotation = _rightHand.rotation * Quaternion.Inverse(weaponObj.transform.rotation);
            weaponObj.transform.rotation = rotation * weaponObj.transform.rotation;

            var displacement = _rightHand.position - weaponObj.transform.position;
            weaponObj.transform.position += displacement;

            if (CurrentWeapon != null)
            {
                Destroy(CurrentWeapon.WeaponGameObject);
            }
            _weaponObject = weapon.WeaponGameObject;
            CurrentWeapon = weapon;
        }
    }
}
