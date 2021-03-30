using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.Utility
{
    [CustomEditor(typeof(GamePropertyConfiguration))]
    class GamePropertyConfigurationEditor : Editor
    {
        bool HasConflict => serializedObject.FindProperty("_hasConflict").boolValue;
        string ConflictName => serializedObject.FindProperty("_conflictName").stringValue;

        GamePropertyConfiguration Properties => target as GamePropertyConfiguration;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            if (HasConflict)
            {
                EditorGUILayout.HelpBox($"Conflict property name \"{ConflictName}\" existing!", MessageType.Error);
            }
            else
            {
                var strQuery = from property in Properties
                               select $"[ \"{property.propertyName}\" => {property.propertyValue} {(property.isReadonly ? " ,Readonly" : null)} ]";
                EditorGUILayout.HelpBox($"PropertyCount: {Properties.Count}.\n" +
                    $"Properties: \n\t{string.Join("\n\t", strQuery)}",
                    MessageType.Info);
            }
        }
    }
}
