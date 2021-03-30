using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya.ItemSystem
{
    [CustomEditor(typeof(TiyaItemManager))]
    public class TiyaItemManagerEditor : Editor
    {
        readonly List<TiyaItem> _dirtyTiyaItemList = new List<TiyaItem>();

        SerializedProperty _itemListSerializedProperty;
        SerializedProperty ItemListSerializedProperty
        {
            get
            {
                if (_itemListSerializedProperty == null)
                {
                    _itemListSerializedProperty = serializedObject.FindProperty("_items");
                }
                return _itemListSerializedProperty;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(ItemListSerializedProperty);

                if (scope.changed)
                {
                    RefreshTiyaItemManager();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshTiyaItemManager()
        {
            foreach (var item in _dirtyTiyaItemList)
            {
                var itemSerializedObject = new SerializedObject(item);
                itemSerializedObject.FindProperty("_itemID").intValue = -1;

                itemSerializedObject.ApplyModifiedProperties();
            }

            _dirtyTiyaItemList.Clear();

            for (int i = 0; i < ItemListSerializedProperty.arraySize; i++)
            {
                var itemValue = (TiyaItem)ItemListSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                if (itemValue != null)
                {
                    _dirtyTiyaItemList.Add(itemValue);

                    var itemSerializedObject = new SerializedObject(itemValue);
                    itemSerializedObject.FindProperty("_itemID").intValue = i + 1;

                    itemSerializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
