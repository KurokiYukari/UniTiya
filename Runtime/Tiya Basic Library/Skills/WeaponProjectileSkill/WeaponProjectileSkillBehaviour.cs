using UnityEngine;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Weapon Projectile Skill")]
    public class WeaponProjectileSkillBehaviour : SkillRefBehaviourBase<WeaponProjectileSkill>
    {
        [SerializeField] WeaponProjectileSkill _skill;
        public override WeaponProjectileSkill Skill => _skill;
    }
}
