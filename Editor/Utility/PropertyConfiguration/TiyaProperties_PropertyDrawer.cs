using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.IntProperty))]
    class IntProperty_PropertyDrawer : PropertyDrawer
    {
        readonly float _labelWidth = 60;
        readonly float _triggerFieldWidth = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;
                position = EditorGUI.PrefixLabel(position, label);

                var totalWidth = position.width;
                var initialXPos = position.x;

                var valueSerializedProperty = property.FindPropertyRelative("value");
                var readonlySerializedProperty = property.FindPropertyRelative("isReadonly");

                position.width = _labelWidth;
                EditorGUI.LabelField(position, new GUIContent("Readonly"));

                position.x += position.width;
                position.width = _triggerFieldWidth;
                readonlySerializedProperty.boolValue = EditorGUI.Toggle(position, readonlySerializedProperty.boolValue);

                position.x += position.width;
                position.width = totalWidth - (position.x - initialXPos);
                valueSerializedProperty.intValue = EditorGUI.IntField(position, valueSerializedProperty.intValue);
            }
        }
    }

    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.StringProperty))]
    class StringProperty_PropertyDrawer : PropertyDrawer
    {
        readonly float _labelWidth = 60;
        readonly float _triggerFieldWidth = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;
                position = EditorGUI.PrefixLabel(position, label);

                var totalWidth = position.width;
                var initialXPos = position.x;

                var valueSerializedProperty = property.FindPropertyRelative("value");
                var readonlySerializedProperty = property.FindPropertyRelative("isReadonly");

                position.width = _labelWidth;
                EditorGUI.LabelField(position, new GUIContent("Readonly"));

                position.x += position.width;
                position.width = _triggerFieldWidth;
                readonlySerializedProperty.boolValue = EditorGUI.Toggle(position, readonlySerializedProperty.boolValue);
                
                position.x += position.width;
                position.width = totalWidth - (position.x - initialXPos);
                valueSerializedProperty.stringValue = EditorGUI.TextField(position, valueSerializedProperty.stringValue);
            }
        }
    }

    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.FloatProperty))]
    class FloatProperty_PropertyDrawer : PropertyDrawer
    {
        readonly float _labelWidth = 60;
        readonly float _triggerFieldWidth = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;
                position = EditorGUI.PrefixLabel(position, label);

                var totalWidth = position.width;
                var initialXPos = position.x;

                var valueSerializedProperty = property.FindPropertyRelative("value");
                var readonlySerializedProperty = property.FindPropertyRelative("isReadonly");

                position.width = _labelWidth;
                EditorGUI.LabelField(position, new GUIContent("Readonly"));

                position.x += position.width;
                position.width = _triggerFieldWidth;
                readonlySerializedProperty.boolValue = EditorGUI.Toggle(position, readonlySerializedProperty.boolValue);

                position.x += position.width;
                position.width = totalWidth - (position.x - initialXPos);
                valueSerializedProperty.floatValue = EditorGUI.FloatField(position, valueSerializedProperty.floatValue);
            }
        }
    }

    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.UnityObjectProperty))]
    class UnityObjectProperty_PropertyDrawer : PropertyDrawer
    {
        readonly float _labelWidth = 60;
        readonly float _triggerFieldWidth = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;
                position = EditorGUI.PrefixLabel(position, label);

                var totalWidth = position.width;
                var initialXPos = position.x;

                var valueSerializedProperty = property.FindPropertyRelative("value");
                var readonlySerializedProperty = property.FindPropertyRelative("isReadonly");

                position.width = _labelWidth;
                EditorGUI.LabelField(position, new GUIContent("Readonly"));

                position.x += position.width;
                position.width = _triggerFieldWidth;
                readonlySerializedProperty.boolValue = EditorGUI.Toggle(position, readonlySerializedProperty.boolValue);

                position.x += position.width;
                position.width = totalWidth - (position.x - initialXPos);
                valueSerializedProperty.objectReferenceValue = EditorGUI.ObjectField(position, valueSerializedProperty.objectReferenceValue, typeof(Object), false);
            }
        }
    }
}
