using System;
using System.Collections.Generic;
using System.Linq;
using Awakening.Input;
using Awakening.Items;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Inventory
{
    /// <summary>
    /// Core Player Inventory System managing item storage, auto-stacking, item consumption, dropping, and sorting.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int _capacity = 20;

        [Header("Starting Items")]
        [SerializeField] private bool _grantStartingPotions = true;

        public int Capacity => _capacity;
        public IReadOnlyList<InventorySlot> Slots => _slots;
        public bool IsOpen { get; private set; } = false;

        public event Action OnInventoryChanged;
        public event Action<bool> OnInventoryToggled;

        private List<InventorySlot> _slots = new List<InventorySlot>();
        private IInputProvider _inputProvider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeSlots();
        }

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;
            if (_inputProvider != null)
            {
                _inputProvider.OnInventoryToggle += ToggleInventory;
            }

            if (_grantStartingPotions)
            {
                AddItem(ItemData.CreateHealthPotionPreset(), 3);
                AddItem(ItemData.CreateManaPotionPreset(), 3);
            }
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnInventoryToggle -= ToggleInventory;
            }
        }

        private void InitializeSlots()
        {
            _slots.Clear();
            for (int i = 0; i < _capacity; i++)
            {
                _slots.Add(new InventorySlot());
            }
        }

        public void ToggleInventory()
        {
            IsOpen = !IsOpen;
            OnInventoryToggled?.Invoke(IsOpen);

            // Manage Cursor state
            Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsOpen;

            Debug.Log($"<color=#00FFAA>[Inventory]</color> Inventory toggled: <b>{(IsOpen ? "OPEN" : "CLOSED")}</b>");
        }

        public void CloseInventory()
        {
            if (!IsOpen) return;
            IsOpen = false;
            OnInventoryToggled?.Invoke(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Attempts to add an item to the inventory with auto-stacking.
        /// </summary>
        public bool AddItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0) return false;

            int remaining = quantity;

            // 1. Pass: Try stacking into existing non-full slots
            if (item.isStackable)
            {
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (!_slots[i].IsEmpty && _slots[i].Item.itemID == item.itemID)
                    {
                        remaining = _slots[i].AddQuantity(remaining);
                        if (remaining <= 0)
                        {
                            OnInventoryChanged?.Invoke();
                            return true;
                        }
                    }
                }
            }

            // 2. Pass: Fill first available empty slots
            while (remaining > 0)
            {
                int emptyIndex = GetFirstEmptySlotIndex();
                if (emptyIndex == -1)
                {
                    Debug.LogWarning("[Inventory] Inventory is FULL! Cannot store remaining items.");
                    OnInventoryChanged?.Invoke();
                    return false;
                }

                int toStore = item.isStackable ? Mathf.Min(item.maxStackSize, remaining) : 1;
                _slots[emptyIndex].Set(item, toStore);
                remaining -= toStore;
            }

            Debug.Log($"<color=#55FF55>[Inventory]</color> Added <b>{quantity}x [{item.rarity}] {item.itemName}</b> to bag.");
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool RemoveItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            if (_slots[slotIndex].IsEmpty) return false;

            _slots[slotIndex].RemoveQuantity(quantity);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            InventorySlot slot = _slots[slotIndex];
            if (slot.IsEmpty || slot.Item == null) return false;

            ItemData item = slot.Item;

            if (item.itemType == ItemType.Consumable)
            {
                bool used = false;

                if (item.restoreHealthAmount > 0 && PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.Heal(item.restoreHealthAmount);
                    used = true;
                }

                if (item.restoreManaAmount > 0 && PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.RestoreMana(item.restoreManaAmount);
                    used = true;
                }

                if (used)
                {
                    Debug.Log($"<color=#00D4FF>[Inventory] Consumed 1x <b>{item.itemName}</b>!</color>");
                    slot.RemoveQuantity(1);
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            Debug.LogWarning($"[Inventory] Item '{item.itemName}' cannot be directly consumed.");
            return false;
        }

        public bool DropItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            InventorySlot slot = _slots[slotIndex];
            if (slot.IsEmpty || slot.Item == null) return false;

            ItemData item = slot.Item;
            int dropQty = Mathf.Min(slot.Quantity, quantity);

            // Spawn WorldItemPickup in front of player
            Transform player = transform;
            Vector3 spawnPos = player.position + player.forward * 1.5f + Vector3.up * 0.4f;

            GameObject pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObj.name = $"Drop_{item.itemName.Replace(" ", "_")}";
            pickupObj.transform.position = spawnPos;
            pickupObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            WorldItemPickup pickup = pickupObj.AddComponent<WorldItemPickup>();
            pickup.Setup(item, dropQty);

            slot.RemoveQuantity(dropQty);
            Debug.Log($"<color=#FFAA00>[Inventory] Dropped {dropQty}x <b>{item.itemName}</b> into the world.</color>");

            OnInventoryChanged?.Invoke();
            return true;
        }

        public void SortInventory()
        {
            // Collect all items
            List<(ItemData item, int qty)> items = new List<(ItemData, int)>();
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty)
                {
                    items.Add((slot.Item, slot.Quantity));
                }
            }

            // Clear slots
            InitializeSlots();

            // Order by ItemType, Rarity, and Name
            var sorted = items
                .OrderBy(x => x.item.itemType)
                .ThenByDescending(x => x.item.rarity)
                .ThenBy(x => x.item.itemName)
                .ToList();

            foreach (var entry in sorted)
            {
                AddItem(entry.item, entry.qty);
            }

            Debug.Log("<color=#00FFAA>[Inventory] Inventory sorted and consolidated!</color>");
            OnInventoryChanged?.Invoke();
        }

        public void ClearInventory()
        {
            InitializeSlots();
            OnInventoryChanged?.Invoke();
        }

        public int GetFirstEmptySlotIndex()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].IsEmpty) return i;
            }
            return -1;
        }
    }
}
