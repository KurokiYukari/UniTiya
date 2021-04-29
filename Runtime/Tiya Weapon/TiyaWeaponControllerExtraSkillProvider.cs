using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using Sarachan.UniTiya.Skill;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaWeapon
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Weapon/Tiya Weapon Controller - Extra Skill Provider")]
    public class TiyaWeaponControllerExtraSkillProvider : MonoBehaviour
    {
        [TypeRestriction(typeof(ISkill))]
        [SerializeField] Object _skillObject;
        [SerializeField] InputAction _skillPerformTrigger;

        ISkill _skill;
        public ISkill Skill
        {
            get
            {
                if (_skillObject == null)
                {
                    throw new MissingReferenceException($"ExtraSkillBind's {nameof(_skillObject)} field can't be null.");
                }
                return _skill ??= _skillObject.ConvertTo<ISkill>();
            }
        }

        public InputAction SkillPerformTrigger
        {
            get
            {
                if (_skillPerformTrigger.bindings.Count == 0)
                {
                    _skillPerformTrigger = null;
                }
                return _skillPerformTrigger;
            }
        }
    }
}
