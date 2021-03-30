using UnityEditor;

namespace Sarachan.UniTiya.ItemSystem
{
    public class TiyaItemEditorBase : Editor
    {
        SerializedProperty _itemIDSerializedProperty;
        protected SerializedProperty ItemIDSerializedProperty => 
            _itemIDSerializedProperty ?? (_itemIDSerializedProperty = serializedObject.FindProperty("_itemID"));

        SerializedProperty _itemNameSerializedProperty;
        protected SerializedProperty ItemNameSerializedProperty => 
            _itemNameSerializedProperty ?? (_itemNameSerializedProperty = serializedObject.FindProperty("_itemName"));

        SerializedProperty _descriptionSerializedProperty;
        protected SerializedProperty DescriptionSerializedProperty =>
            _descriptionSerializedProperty ?? (_descriptionSerializedProperty = serializedObject.FindProperty("_description"));

        SerializedProperty _isStackableSerializedProperty;
        protected SerializedProperty IsStackableSerializedProperty =>
            _isStackableSerializedProperty ?? (_isStackableSerializedProperty = serializedObject.FindProperty("_isStackable"));

        SerializedProperty _itemTypeSerializedProperty;
        protected SerializedProperty ItemTypeSerializedProperty =>
            _itemTypeSerializedProperty ?? (_itemTypeSerializedProperty = serializedObject.FindProperty("_itemType"));

        SerializedProperty _customPropertiesSerializedProperty;
        protected SerializedProperty CustionPropertiesSerializedProperty =>
            _customPropertiesSerializedProperty ?? (_customPropertiesSerializedProperty = serializedObject.FindProperty("_itemExtraProperties"));

        SerializedProperty _itemEventSerializedProperty;
        protected SerializedProperty ItemEventSerializedProperty =>
            _itemEventSerializedProperty ?? (_itemEventSerializedProperty = serializedObject.FindProperty("_onUsingItem"));
    }
}
