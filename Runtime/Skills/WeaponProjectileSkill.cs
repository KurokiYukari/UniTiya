using UnityEngine;
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

        void PerformAction()
        {
            Vector3 direction = Weapon.Owner.GetAimFocusPosition() - _projectPoint.position;

            //Debug.DrawRay(ProjectPoint.position, direction, Color.blue, 5f);

            // TODO: 执行精度误差计算
            // 或者这个计算也可以直接用在前面？（在准星内随机一个点作为方向

            GameObject projectileObj = TiyaGameSystem.Pool.InstantiatePrefab(_projectilePrefabID, _projectPoint.position, Quaternion.LookRotation(direction));
            RecentProjectileObject = projectileObj;
            projectileObj.GetComponent<Rigidbody>().velocity = InitialSpeed * projectileObj.transform.forward;
            projectileObj.GetComponent<TiyaProjectile>().Weapon = Weapon;

            //weapon.CurrentAccuracy = Mathf.Clamp(weapon.CurrentAccuracy + weapon._accuracyDropPerShot, weapon._accuracy, weapon._maxAccuracy);

            // MuzzleEffect
            //if (weapon._enableEffect)
            //{
            //    weapon._muzzleEffectInitializer.InstantiatePrefab(_projectPoint.position, Quaternion.LookRotation(direction));
            //}
        }

        protected override void OnInit()
        {
            ShotCDConsumer = new ColdDownConsumer(ShotColdDownTime);
            SkillConsumers.ConsumerList.Add(ShotCDConsumer);

            OnPerforming += PerformAction;
        }
    }

    public enum ProjectileMode
    {
        Single = 1,
        Triple = 1 << 1,
        Auto = 1 << 2,
    }
}
