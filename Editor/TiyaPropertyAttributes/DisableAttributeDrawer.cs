using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    class DisableAttributeDrawer : PropertyDrawer
    {
        DisableAttribute DisableAttribute => attribute as DisableAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var disabled = true;

            if (DisableAttribute.ConditionFieldName != null)
            {
                var conditionFieldSerializedProperty = property.FindPropertyRelative(DisableAttribute.ConditionFieldName);
                if (conditionFieldSerializedProperty == null)
                {
                    Debug.LogWarning($"Can't find serialized field named {DisableAttribute.ConditionFieldName}");
                }
                else
                {
                    disabled = conditionFieldSerializedProperty.boolValue == DisableAttribute.DisableIfCondition;
                }
            }

            using (new EditorGUI.DisabledGroupScope(disabled))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
