using UnityEditor;

namespace Sarachan.UniTiya.Skill
{
    [CustomEditor(typeof(WeaponProjectileSkillBehaviour))]
    class WeaponProjectileSkillEditor : Editor
    {
        private readonly System.Type[] _avaliableExtensions = new System.Type[]
        {
            typeof(WeaponProjectileSkill_RecoilExtensioin),
            typeof(WeaponProjectileSkill_BiasExtension)
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.AddExtensionField("Extension", null, _avaliableExtensions);
        }
    }
}
