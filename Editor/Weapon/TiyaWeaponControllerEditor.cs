//using System.Reflection;
//using UnityEditor;
//using UnityEngine;
//using UnityEditorInternal;
//using UnityEngine.InputSystem;

//namespace Sarachan.UniTiya.TiyaWeapon
//{
//    [CustomEditor(typeof(TiyaWeaponController))]
//    class TiyaWeaponControllerEditor : Editor
//    {
//        SerializedProperty _actorWeaponActionsSerializedProperty;
//        SerializedProperty _normalSkillSerializedProperty;
//        SerializedProperty _specialSkillSerializedProperty;
//        SerializedProperty _skillBindArraySerializedProperty;
//        SerializedProperty _aimModeSerialziedProperty;

//        ReorderableList _skillBindsList;

//        private void OnEnable()
//        {
//            _actorWeaponActionsSerializedProperty = serializedObject.FindProperty("_actorWeaponActionsOverride");
//            _normalSkillSerializedProperty = serializedObject.FindProperty("_normalAttackSkillObject");
//            _specialSkillSerializedProperty = serializedObject.FindProperty("_specialAttackSkillObject");
//            _skillBindArraySerializedProperty = serializedObject.FindProperty("_extraSkillBinds");
//            _aimModeSerialziedProperty = serializedObject.FindProperty("_actorAimMode");

//            _skillBindsList = new ReorderableList(serializedObject, _skillBindArraySerializedProperty)
//            {
//                onAddCallback = AddListener,

//                drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
//                {
//                    EditorGUI.PropertyField(rect, _skillBindArraySerializedProperty.GetArrayElementAtIndex(index), true);
//                },

//                elementHeightCallback = (int index) =>
//                {
//                    return EditorGUI.GetPropertyHeight(_skillBindArraySerializedProperty.GetArrayElementAtIndex(index), true);
//                }
//            };

//            void AddListener(ReorderableList list)
//            {
//                list.serializedProperty.arraySize++;
//                list.index = list.serializedProperty.arraySize - 1;
//                list.serializedProperty.serializedObject.ApplyModifiedProperties();

//                var skillBindSerializedProperty = _skillBindArraySerializedProperty.GetArrayElementAtIndex(list.index);

//                var skillBind = skillBindSerializedProperty.GetTargetObject();
//                var field = skillBind.GetType()
//                    .GetField("_skillPerformTrigger", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//                field?.SetValue(skillBind, new InputAction());

//                skillBindSerializedProperty.SetTargetObjectOfProperty(skillBind);

//                list.serializedProperty.serializedObject.ApplyModifiedProperties();
//            }
//        }

//        public override void OnInspectorGUI()
//        {
//            serializedObject.Update();

//            EditorGUILayout.PropertyField(_actorWeaponActionsSerializedProperty);
//            EditorGUILayout.PropertyField(_normalSkillSerializedProperty);
//            EditorGUILayout.PropertyField(_specialSkillSerializedProperty);

//            _skillBindsList.DoLayoutList();

//            EditorGUILayout.PropertyField(_aimModeSerialziedProperty);

//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//}