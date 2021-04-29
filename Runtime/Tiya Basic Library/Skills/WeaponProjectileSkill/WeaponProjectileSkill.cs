using UnityEngine;

using UniRx;

using Sarachan.UniTiya.Consumer;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// SkillPerformer 需要是 IWeaponController 的 GameObject。
    /// 向 Weapon Owner 的 AimPose 方向投掷一个 <see cref="TiyaProjectile"/> 的 Skill。
    /// </summary>
    [System.Serializable]
    public class WeaponProjectileSkill : TiyaSkill
    {
        [Tooltip("两次射击的最小时间间隔。")]
        [SerializeField, SetProperty(nameof(ShotColdDownTime))] private float _shotColdDownTime = 0.1f;

        [Header("Projectile")]
        [SerializeField] private string _projectilePrefabID;
        [SerializeField] private float _initialSpeed;
        [SerializeField] private Transform _projectPoint;

        [Disable]
        [SerializeField] float _currentBias;

        private IWeaponController _weapon = null;
        public IWeaponController Weapon => _weapon ??= SkillPerformer.GetComponent<IWeaponController>();

        public float ShotColdDownTime
        {
            get => _shotColdDownTime;
            set
            {
                _shotColdDownTime = value;
                if (ShotCDConsumer != null)
                {
                    ShotCDConsumer.ColdDownTime = value;
                }
            }
        }
        public ColdDownConsumer ShotCDConsumer { get; private set; }

        public float InitialSpeed { get => _initialSpeed; set => _initialSpeed = value; }

        public Transform ProjectPoint => _projectPoint;

        GameObject _recentProjectileObject;
        public GameObject RecentProjectileObject
        {
            get
            {
                try
                {
                    if (_recentProjectileObject.activeInHierarchy)
                    {
                        return _recentProjectileObject;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (MissingReferenceException)
                {
                    return null;
                }
            }
            private set => _recentProjectileObject = value;
        }

        /// <summary>
        /// 当前的精度偏差
        /// </summary>
        public float CurrentBias { get => _currentBias; set => _currentBias = value; }

        void PerformAction()
        {
            // TODO: 执行 CurrentBias 精度误差计算
            // 在准星内随机一个点作为方向？给 GetAimFocusPosition 添加精度参数？

            Vector3 direction = Weapon.Owner.GetAimFocusPosition() - _projectPoint.position;

            //Debug.DrawRay(ProjectPoint.position, direction, Color.blue, 5f);

            GameObject projectileObj = TiyaGameSystem.Pool.InstantiatePrefab(_projectilePrefabID, _projectPoint.position, Quaternion.LookRotation(direction));
            RecentProjectileObject = projectileObj;
            var tiyaProjectile = projectileObj.GetComponent<TiyaProjectile>();
            tiyaProjectile.Weapon = Weapon;
            tiyaProjectile.DamageEnabler.Enable();

            //Observable.NextFrame()
            //    .Subscribe(_ => projectileObj.GetComponent<Rigidbody>().velocity = InitialSpeed * projectileObj.transform.forward);
            projectileObj.GetComponent<Rigidbody>().velocity = InitialSpeed * projectileObj.transform.forward;

            // MuzzleEffect
            //if (weapon._enableEffect)
            //{
            //    weapon._muzzleEffectInitializer.InstantiatePrefab(_projectPoint.position, Quaternion.LookRotation(direction));
            //}
        }

        protected override bool OnInit()
        {
            ShotCDConsumer = new ColdDownConsumer(ShotColdDownTime);
            SkillConsumers.ConsumerList.Add(ShotCDConsumer);

            OnPerforming += PerformAction;
            return true;
        }
    }

    public enum ProjectileMode
    {
        Single = 1,
        Triple = 1 << 1,
        Auto = 1 << 2,
    }
}
