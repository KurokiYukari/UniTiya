using UnityEngine;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(WeaponProjectileSkillBehaviour))]
    public class WeaponProjectileSkill_BiasExtension : MonoBehaviour
    {
        [SerializeField] float _defaultBias = 10;
        [SerializeField] float _maxBias = 100;

        [SerializeField] float _biasIncreasePerPerform = 30;
        [SerializeField] float _biasRecoverRatio = 0.3f;

        WeaponProjectileSkillBehaviour _projectileSkillBehaviour;
        public WeaponProjectileSkill ProjectileSkill => _projectileSkillBehaviour.Skill;

        protected void Awake()
        {
            _projectileSkillBehaviour = GetComponent<WeaponProjectileSkillBehaviour>();
        }

        protected void Start()
        {
            ProjectileSkill.CurrentBias = _defaultBias;
        }

        protected void OnEnable()
        {
            ProjectileSkill.OnPerforming += OnPerformListener;
        }

        protected void OnDisable()
        {
            ProjectileSkill.OnPerforming -= OnPerformListener;
        }

        protected void Update()
        {
            ProjectileSkill.CurrentBias = Mathf.Lerp(ProjectileSkill.CurrentBias, _defaultBias, _biasRecoverRatio);
        }

        void OnPerformListener()
        {
            ProjectileSkill.CurrentBias = Mathf.Clamp(ProjectileSkill.CurrentBias + _biasIncreasePerPerform, _defaultBias, _maxBias);
        }
    }
}
