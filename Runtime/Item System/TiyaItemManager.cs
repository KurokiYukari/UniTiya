using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.ItemSystem
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Tiya Item Manager")]
    public class TiyaItemManager : ScriptableObject, IItemManager
    {
        [SerializeField] List<TiyaItem> _items;

        public IItem this[int itemID] => GetItemByID(itemID);

        public IItem GetItemByID(int itemID) => _items[itemID];

        public IItem CreateItemByID(int itemID)
        {
            var targetItem = _items[itemID];
            if (targetItem.IsStackable)
            {
                return targetItem;
            }
            else
            {
                return Instantiate(targetItem);
            }
        }
    }
}
