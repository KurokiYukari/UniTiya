using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.Skill
{
    [CustomEditor(typeof(TiyaProjectile))]
    public class ProjectileEditor : Editor
    {
        private readonly System.Type[] _avaliableExtensions = new System.Type[]
        {
            typeof(Utility.TimedObjRecycler)
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.AddExtensionField("Extension", null, _avaliableExtensions);
        }
    }
}
