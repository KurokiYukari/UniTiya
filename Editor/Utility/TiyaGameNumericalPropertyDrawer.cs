using UnityEditor;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    //[CustomPropertyDrawer(typeof(TiyaGameDynamicNumericalProperty))]
    [CustomPropertyDrawer(typeof(TiyaGameFixedNumericalProperty))]
    class TiyaGameNumericalPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var allowNegativeSerializedProperty = property.FindPropertyRelative("_allowNegativeValue");
            var baseValueSerializedProperty = property.FindPropertyRelative("_baseValue");

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position = EditorGUI.PrefixLabel(position, label);

                var valueFieldRect = new Rect(position) { width = position.width / 2 };
                baseValueSerializedProperty.floatValue = EditorGUI.FloatField(valueFieldRect, new GUIContent("", "Base Value"), baseValueSerializedProperty.floatValue);

                var allowNegativeFieldRect = new Rect(valueFieldRect)
                {
                    x = valueFieldRect.x + valueFieldRect.width + 5,
                    width = position.width - valueFieldRect.width - 5
                };
                var toggleLabelRect = new Rect(allowNegativeFieldRect) { width = allowNegativeFieldRect.width - 20 - 5 };
                EditorGUI.LabelField(allowNegativeFieldRect, "Allow Negative");
                var togglePosition = new Rect(toggleLabelRect) { width = 20, x = toggleLabelRect.x + toggleLabelRect.width + 5 };
                allowNegativeSerializedProperty.boolValue = EditorGUI.Toggle(togglePosition, allowNegativeSerializedProperty.boolValue);
            }
        }
    }
}