using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    class HideIfAttributeDrawer : PropertyDrawer
    {
        HideIfAttribute HideIfAttribute => attribute as HideIfAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!CheckHideIf(property))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!CheckHideIf(property))
            {
                return base.GetPropertyHeight(property, label);
            }
            else
            {
                return 0;
            }
        }

        bool CheckHideIf(SerializedProperty property)
        {
            var hided = true;

            var conditionFieldSerializedProperty = property.serializedObject.FindProperty(HideIfAttribute.HideConditionFieldName);
            if (conditionFieldSerializedProperty == null)
            {
                Debug.LogWarning($"Can't find serialized field named {HideIfAttribute.HideConditionFieldName}");
            }
            else
            {
                hided = conditionFieldSerializedProperty.boolValue == HideIfAttribute.HideIfCondition;
            }

            return hided;
        }
    }
}
