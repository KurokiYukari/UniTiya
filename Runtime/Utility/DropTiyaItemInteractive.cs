using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Interaction
{
    /// <summary>
    /// 将指定物体加入角色背包的 Interactive
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Interaction/Drop Item Interactive")]
    public class DropTiyaItemInteractive : TiyaInteractive
    {
        [System.Serializable]
        struct TiyaItemCountPair
        {
            [SerializeField] [TypeRestriction(typeof(IItem))] Object _itemObject;
            [SerializeField] int _count;

            IItem _item;
            public IItem Item => _item ??= _itemObject.ConvertTo<IItem>();
            public int Count => _count;
        }
        [SerializeField] List<TiyaItemCountPair> _serializedItems;

        private readonly List<(IItem item, int itemCout)> _items = new List<(IItem item, int itemCout)>();
        public List<(IItem item, int itemCout)> Items => _items;

        protected void Start()
        {
            Items.AddRange(from pair in _serializedItems select ((IItem)pair.Item, pair.Count));
        }

        protected override void InteractOverride(IActorController actor)
        {
            var actorInventory = actor.GameObject.GetComponent<IInventory>();

            foreach (var (item, itemCout) in Items)
            {
                actorInventory.ChangeInventoryItem(item, itemCout);
            }

            foreach (var handler in ConnettedHandlers)
            {
                handler.Interactives.Remove(this);
            }
            Destroy(gameObject);
        }

        public static DropTiyaItemInteractive CreateDefaultInteractiveObject
            (Vector3 position, IEnumerable<(IItem item, int itemCout)> dropItems = null, float radius = 4, Vector3 triggerCenterOffset = default)
        {
            var interactiveObject = Instantiate(new GameObject());
            interactiveObject.transform.position = position;

            var sphereCollider = interactiveObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;
            sphereCollider.center = triggerCenterOffset;

            var dropItemInteractive = interactiveObject.AddComponent<DropTiyaItemInteractive>();
            dropItemInteractive.Items.AddRange(dropItems);
            return dropItemInteractive;
        }
    }
}
