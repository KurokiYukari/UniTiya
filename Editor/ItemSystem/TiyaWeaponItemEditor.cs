using UnityEditor;

namespace Sarachan.UniTiya.ItemSystem
{
    [CustomEditor(typeof(TiyaWeaponItem))]
    class TiyaWeaponItemEditor : TiyaItemEditorBase
    {
        SerializedProperty _weaponPrefabSerializedProperty;
        protected SerializedProperty WeaponPrefabSerializedProperty =>
            _weaponPrefabSerializedProperty ?? (_weaponPrefabSerializedProperty = serializedObject.FindProperty("_weaponPrefab"));

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Toggle("Weapon", true);

            EditorGUILayout.PropertyField(ItemIDSerializedProperty);
            EditorGUILayout.PropertyField(ItemNameSerializedProperty);
            EditorGUILayout.PropertyField(DescriptionSerializedProperty);
            EditorGUILayout.PropertyField(WeaponPrefabSerializedProperty);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                IsStackableSerializedProperty.boolValue = false;
                ItemTypeSerializedProperty.enumValueIndex = (int)TiyaItemType.Equipment;

                EditorGUILayout.PropertyField(IsStackableSerializedProperty);
                EditorGUILayout.PropertyField(ItemTypeSerializedProperty);
            }

            EditorGUILayout.PropertyField(CustionPropertiesSerializedProperty);
            EditorGUILayout.PropertyField(ItemEventSerializedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
