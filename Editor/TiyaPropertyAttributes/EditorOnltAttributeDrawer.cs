using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    [CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
    class EditorOnltAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
