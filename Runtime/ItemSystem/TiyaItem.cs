using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.TiyaPropertyAttributes;
using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.ItemSystem
{
    /// <summary>
    /// 可配置的 Item。
    /// </summary>
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Tiya Item")]
    public class TiyaItem : ScriptableObject, IItem, ISerializationCallbackReceiver
    {
        [Disable]
        [SerializeField] int _itemID = -1;

        [SerializeField] string _itemName;
        [SerializeField] [TextArea] string _description;
        [SerializeField] bool _isStackable;
        [SerializeField] TiyaItemType _itemType;

        [SerializeField] GamePropertyConfiguration _itemExtraProperties;

        [SerializeField] TiyaItemEvent _onUsingItem;

        public int ItemID => _itemID;
        public string ItemName => _itemName;
        public string Description => _description;
        public bool IsStackable => _isStackable;
        public TiyaItemType ItemType => _itemType;

        public IGameProperties Properties { get; } = new RuntimePropertyConfiguration();
        
        public object this[string propertyName]
        {
            get => Properties[propertyName];
            set => Properties[propertyName] = value;
        }

        public event System.Action<IActorController> OnUsingItem;

        public void UseItem(IActorController itemOwner)
        {
            OnUsingItem?.Invoke(itemOwner);
            _onUsingItem.Invoke(this, itemOwner);
            UseItemOverride(itemOwner);

            if (_itemType == TiyaItemType.Expendable)
            {
                this.ConsumeItem(itemOwner.Inventory);
            }
        }

        protected virtual void UseItemOverride(IActorController itemOwner) { }

        public T GetProperty<T>(string propertyName) => 
            Properties.GetProperty<T>(propertyName);

        public T GetProperty<T>(string propertyName, out bool isReadonly) =>
            Properties.GetProperty<T>(propertyName, out isReadonly);

        public bool AddProperty(string propertyName, object value, bool isReadOnly = false) =>
            Properties.AddProperty(propertyName, value, isReadOnly);

        public void SetProperty(string propertyName, object value, bool isReadOnly = false) =>
            Properties.SetProperty(propertyName, value, isReadOnly);

        public bool RemoveProperty(string propertyName) =>
            Properties.RemoveProperty(propertyName);

        public bool ContainsProperty(string propertyName) =>
            Properties.ContainsProperty(propertyName);

        public bool IsReadonlyProperty(string propertyName) => 
            Properties.IsReadonlyProperty(propertyName);

        public IEnumerator<(string propertyName, object propertyValue, bool isReadonly)> GetEnumerator() =>
            Properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Properties.GetEnumerator();

        public virtual void OnBeforeSerialize()
        {
            try
            {
                _itemID = GetProperty<int>(nameof(ItemID));
                _itemName = GetProperty<string>(nameof(ItemName));
                _description = GetProperty<string>(nameof(Description));
                _isStackable = GetProperty<bool>(nameof(IsStackable));
                _itemType = GetProperty<TiyaItemType>(nameof(ItemType));
            }
            catch (KeyNotFoundException)
            {
            }
        }

        public virtual void OnAfterDeserialize()
        {
            SetProperty(nameof(ItemID), _itemID, true);
            SetProperty(nameof(ItemName), _itemName, true);
            SetProperty(nameof(Description), _description, true);
            SetProperty(nameof(IsStackable), _isStackable, true);
            SetProperty(nameof(ItemType), _itemType, true);
        }

        [System.Serializable]
        private class TiyaItemEvent : UnityEvent<IItem, IActorController> { }

        public class TiyaItemComparer : IEqualityComparer<IItem>
        {
            public bool Equals(IItem x, IItem y) => x.ItemEquals(y);

            public int GetHashCode(IItem obj)
            {
                if (obj.IsStackable)
                {
                    return obj.ItemID.GetHashCode();
                }
                else
                {
                    return obj.GetHashCode();
                }
            }

            static TiyaItemComparer _instance;
            public static TiyaItemComparer Instance => _instance ??= new TiyaItemComparer();
        }

        protected void OnEnable()
        {
            if (_itemExtraProperties != null)
            {
                foreach (var (propertyName, propertyValue, isReadonly) in _itemExtraProperties)
                {
                    SetProperty(propertyName, propertyValue, isReadonly);
                }
            }
        }
    }
}
