using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    [CustomPropertyDrawer(typeof(TypeRestrictionAttribute))]
    class TypeRestrictionAttributeDrawer : PropertyDrawer
    {
        public TypeRestrictionAttribute TypeRestrictionAttribute => attribute as TypeRestrictionAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.PropertyField(position, property, label);

                if (scope.changed)
                {
                    if (!CheckObjectType(property.objectReferenceValue))
                    {
                        property.objectReferenceValue = null;
                        Debug.LogError($"Can't find {TypeRestrictionAttribute.RestrictedType.FullName} from field {property.name}.");
                    }
                }
            }
        }

        bool CheckObjectType(Object obj)
        {
            if (obj != null)
            {
                if (TypeRestrictionAttribute.RestrictedType.IsAssignableFrom(obj.GetType()))
                {
                    return true;
                }

                var gameObjectValue = obj as GameObject;
                if (gameObjectValue != null && gameObjectValue.GetComponent(TypeRestrictionAttribute.RestrictedType) != null)
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
