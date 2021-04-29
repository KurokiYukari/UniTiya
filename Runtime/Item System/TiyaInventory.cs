using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.Interaction;

namespace Sarachan.UniTiya.ItemSystem
{
    /// <summary>
    /// IInventory 的实现
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Item System/Tiya Inventory")]
    public class TiyaInventory : MonoBehaviour, IInventory
    {
        [SerializeField] ChangeInventoryItemEvent _onChangeInventoryItemUnityEvent;
        public event System.Action<IItem, int> OnChangeInventoryItem;

        readonly HashSet<IItem> _uniqueItems = new HashSet<IItem>(TiyaItem.TiyaItemComparer.Instance);
        readonly Dictionary<IItem, int> _stackableItems = new Dictionary<IItem, int>(TiyaItem.TiyaItemComparer.Instance);

        IActorController _inventoryOwnerActor;
        public IActorController InventoryOwnerActor => _inventoryOwnerActor ??= GetComponent<IActorController>();

        public void ChangeInventoryItem(IItem item, int itemCount)
        {
            if (itemCount == 0)
            {
                return;
            }

            OnChangeInventoryItem(item, itemCount);
            _onChangeInventoryItemUnityEvent.Invoke(item, itemCount);
            if (itemCount > 0)
            {
                AddItem(item, itemCount);
            }
            else
            {
                RemoveItem(item, -itemCount);
            }
        }

        private void AddItem(IItem item, int itemCount = 1)
        {
            if (item.IsStackable)
            {
                if (_stackableItems.ContainsKey(item))
                {
                    _stackableItems[item] += itemCount;
                }
                else
                {
                    _stackableItems[item] = itemCount;
                }
                return;
            }
            else
            {
                _uniqueItems.Add(item);
            }
        }
        private void RemoveItem(IItem item, int itemCount = 1)
        {
            if (item.IsStackable)
            {
                if (_stackableItems.ContainsKey(item))
                {
                    _stackableItems[item] -= itemCount;
                    if (_stackableItems[item] <= 0)
                    {
                        _stackableItems.Remove(item);
                    }
                }
                return;
            }
            else
            {
                _uniqueItems.Remove(item);
            }
        }

        public void AbandonItem(IEnumerable<(IItem item, int itemCount)> items, Vector3? position = null)
        {
            foreach (var (item, itemCount) in items)
            {
                Debug.Assert(HasEnoughItem(item, itemCount));
            }

            foreach (var (item, itemCount) in items)
            {
                ChangeInventoryItem(item, -itemCount);
            }

            var instantiatePosition = position ?? InventoryOwnerActor.ActorTransform.position;
            DropTiyaItemInteractive.CreateDefaultInteractiveObject(instantiatePosition, items);
        }

        public int GetItemCount(IItem item)
        {
            if (!item.IsStackable)
            {
                return _uniqueItems.Contains(item) ? 1 : 0;
            }
            else
            {
                return _stackableItems.ContainsKey(item) ? _stackableItems[item] : 0;
            }
        }

        public bool HasEnoughItem(IItem item, int itemCount = 1)
        {
            return GetItemCount(item) > itemCount;
        }

        [System.Serializable]
        public class ChangeInventoryItemEvent : UnityEvent<IItem, int> { }
    }
}
