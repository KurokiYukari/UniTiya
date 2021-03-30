using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    [CustomPropertyDrawer(typeof(MultiEnumAttribute))]
    class MultiEnumAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
}
