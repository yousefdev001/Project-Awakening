using System;
using Awakening.Items;
using UnityEngine;

namespace Awakening.Inventory
{
    /// <summary>
    /// Represents a single slot in the Player's Inventory.
    /// Supports item storage, stacking, and quantity management.
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity;

        public ItemData Item => _itemData;
        public int Quantity => _quantity;
        public bool IsEmpty => _itemData == null || _quantity <= 0;

        public InventorySlot()
        {
            Clear();
        }

        public InventorySlot(ItemData item, int quantity)
        {
            _itemData = item;
            _quantity = quantity;
        }

        public void Set(ItemData item, int quantity)
        {
            _itemData = item;
            _quantity = quantity;
        }

        public int AddQuantity(int amount)
        {
            if (IsEmpty || _itemData == null) return amount;

            if (!_itemData.isStackable)
            {
                return amount; // Non-stackable cannot take additional
            }

            int spaceAvailable = _itemData.maxStackSize - _quantity;
            int toAdd = Mathf.Min(spaceAvailable, amount);

            _quantity += toAdd;
            int leftover = amount - toAdd;

            return leftover;
        }

        public int RemoveQuantity(int amount)
        {
            if (IsEmpty) return 0;

            int toRemove = Mathf.Min(_quantity, amount);
            _quantity -= toRemove;

            if (_quantity <= 0)
            {
                Clear();
            }

            return toRemove;
        }

        public void Clear()
        {
            _itemData = null;
            _quantity = 0;
        }
    }
}
