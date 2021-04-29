using UnityEditor;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    [CustomPropertyDrawer(typeof(RandomObjctEmitter))]
    class RandomObjctEmitterPropertyDrawer : PropertyDrawer
    {
        const string WEIGHT_PAIR_ARRAY_FIELD_NAME = "_prefabIds";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            var weightPairArraySerializedProperty = property.FindPropertyRelative(WEIGHT_PAIR_ARRAY_FIELD_NAME);
            EditorGUI.PropertyField(position, weightPairArraySerializedProperty, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var weightPairArraySerializedProperty = property.FindPropertyRelative(WEIGHT_PAIR_ARRAY_FIELD_NAME);
            return EditorGUI.GetPropertyHeight(weightPairArraySerializedProperty, true);
        }
    }
}
