using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaView
{
    // TODO: 暂未用到
    /// <summary>
    /// <see cref="TiyaActorCameraView"/> 对 <see cref="IWeaponController"/> 的一些适配，
    /// 为 Weapon 提供对于 TiyaActorView 的设置（所以该组件只在 Weapon owner 为 Player 时生效）。
    /// 该组件应该放在 <see cref="IWeaponController"/> 同一级。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/View/Tiya Actor Camera View - Weapon Adapter")]
    public class TiyaActorCameraView_WeaponAdapter : MonoBehaviour
    {
        [SerializeField] Transform _enableFirstViewCamera;
        [HideIf(nameof(_enableFirstViewCamera), false)]
        [SerializeField] Transform _firstViewCameraFollowOverride;
        Transform _originFirstViewCameraFollow;

        public IWeaponController Weapon { get; private set; }
        public TiyaActorCameraView ActorCameraView { get; set; }

        protected void Awake()
        {
            Weapon = GetComponent<IWeaponController>() ?? throw new MissingComponentException(nameof(IWeaponController));
        }

        protected void OnEnable()
        {
            Weapon.OnEquip += OnEquipListener;
        }

        protected void OnDisable()
        {
            Weapon.OnEquip -= OnEquipListener;

            if (Weapon.Owner != null && Weapon.Owner.IsPlayer)
            {
                if (ActorCameraView)
                {
                    ActorCameraView.FirstViewPlayerCameraFollow = _originFirstViewCameraFollow;
                }
            }
        }

        void OnEquipListener(IActorController owner)
        {
            if (!owner.IsPlayer)
            {
                Destroy(this);
                return;
            }

            var ActorCameraView = owner.ActorView as TiyaActorCameraView;
            if (!ActorCameraView)
            {
                Debug.LogWarning($"{nameof(TiyaActorCameraView_WeaponAdapter)} requires {nameof(TiyaActorCameraView)} used in actorView module, but it is missing.");
                return;
            }

            _originFirstViewCameraFollow = ActorCameraView.FirstViewPlayerCameraFollow;
        }
    }
}
