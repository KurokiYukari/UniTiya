using UnityEngine;
using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(WeaponProjectileSkillBehaviour))]
    public class WeaponProjectileSkill_RecoilExtensioin : MonoBehaviour
    {
        [SerializeField] ActorRecoilTool _recoilSetting;

        WeaponProjectileSkillBehaviour _projectileSkillBehaviour;
        public WeaponProjectileSkill ProjectileSkill => _projectileSkillBehaviour.Skill;

        protected void Awake()
        {
            _projectileSkillBehaviour = GetComponent<WeaponProjectileSkillBehaviour>();
        }

        protected void OnEnable()
        {
            ProjectileSkill.OnPerforming += OnPerformListener;
        }

        protected void OnDisable()
        {
            ProjectileSkill.OnPerforming -= OnPerformListener;
        }

        void OnPerformListener()
        {
            _recoilSetting.DoRecoilTo(ProjectileSkill.Weapon.Owner);
        }
    }
}
