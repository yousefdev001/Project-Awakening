using System;
using Awakening.Inventory;
using Awakening.Items;
using Awakening.Player;
using Awakening.Professions;
using UnityEngine;

namespace Awakening.Equipment
{
    /// <summary>
    /// Manages player equipped gear (Weapon, Armor, Accessory), stat calculations, and Profession Weapon Affinity synergies.
    /// </summary>
    public class EquipmentSystem : MonoBehaviour
    {
        public static EquipmentSystem Instance { get; private set; }

        [Header("Equipped Gear (Read Only)")]
        [SerializeField] private ItemData _equippedWeapon;
        [SerializeField] private ItemData _equippedArmor;
        [SerializeField] private ItemData _equippedAccessory;

        public ItemData EquippedWeapon => _equippedWeapon;
        public ItemData EquippedArmor => _equippedArmor;
        public ItemData EquippedAccessory => _equippedAccessory;

        public bool HasWeaponAffinity { get; private set; } = false;
        public float AffinityDamageMultiplier => HasWeaponAffinity ? 1.20f : 1.0f; // +20% Synergy Boost

        public event Action OnEquipmentChanged;

        private PlayerStats _playerStats;
        private InventorySystem _inventory;
        private ProfessionSystem _professionSystem;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _playerStats = GetComponent<PlayerStats>() ?? PlayerStats.Instance;
            _inventory = GetComponent<InventorySystem>() ?? InventorySystem.Instance;
            _professionSystem = GetComponent<ProfessionSystem>() ?? ProfessionSystem.Instance;
        }

        private void Start()
        {
            AcquireReferences();

            if (_professionSystem != null)
            {
                _professionSystem.OnProfessionAssigned += _ => RecalculateEquipmentBonuses();
            }

            RecalculateEquipmentBonuses();
        }

        private void AcquireReferences()
        {
            if (_playerStats == null) _playerStats = GetComponent<PlayerStats>() ?? PlayerStats.Instance;
            if (_inventory == null) _inventory = GetComponent<InventorySystem>() ?? InventorySystem.Instance;
            if (_professionSystem == null) _professionSystem = GetComponent<ProfessionSystem>() ?? ProfessionSystem.Instance;
        }

        public bool EquipFromInventory(int inventorySlotIndex)
        {
            AcquireReferences();
            if (_inventory == null) return false;

            if (inventorySlotIndex < 0 || inventorySlotIndex >= _inventory.Slots.Count) return false;
            InventorySlot slot = _inventory.Slots[inventorySlotIndex];
            if (slot.IsEmpty || slot.Item == null) return false;

            ItemData newItem = slot.Item;
            if (newItem.equipmentSlot == EquipmentSlotType.None)
            {
                Debug.LogWarning($"[EquipmentSystem] Item '{newItem.itemName}' cannot be equipped.");
                return false;
            }

            ItemData previousItem = null;

            switch (newItem.equipmentSlot)
            {
                case EquipmentSlotType.Weapon:
                    previousItem = _equippedWeapon;
                    _equippedWeapon = newItem;
                    break;
                case EquipmentSlotType.Armor:
                    previousItem = _equippedArmor;
                    _equippedArmor = newItem;
                    break;
                case EquipmentSlotType.Accessory:
                    previousItem = _equippedAccessory;
                    _equippedAccessory = newItem;
                    break;
            }

            // Remove 1 from inventory slot
            slot.RemoveQuantity(1);

            // Return previous item to inventory if existed
            if (previousItem != null)
            {
                _inventory.AddItem(previousItem, 1);
            }

            Debug.Log($"<color=#00FFAA>[EquipmentSystem]</color> Equipped <b>[{newItem.rarity}] {newItem.itemName}</b>!");
            RecalculateEquipmentBonuses();
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public bool UnequipToInventory(EquipmentSlotType slotType)
        {
            AcquireReferences();
            if (_inventory == null) return false;

            ItemData itemToUnequip = null;

            switch (slotType)
            {
                case EquipmentSlotType.Weapon:
                    itemToUnequip = _equippedWeapon;
                    _equippedWeapon = null;
                    break;
                case EquipmentSlotType.Armor:
                    itemToUnequip = _equippedArmor;
                    _equippedArmor = null;
                    break;
                case EquipmentSlotType.Accessory:
                    itemToUnequip = _equippedAccessory;
                    _equippedAccessory = null;
                    break;
            }

            if (itemToUnequip == null) return false;

            bool added = _inventory.AddItem(itemToUnequip, 1);
            if (!added)
            {
                // Restore back if bag full
                if (slotType == EquipmentSlotType.Weapon) _equippedWeapon = itemToUnequip;
                else if (slotType == EquipmentSlotType.Armor) _equippedArmor = itemToUnequip;
                else _equippedAccessory = itemToUnequip;
                Debug.LogWarning("[EquipmentSystem] Cannot unequip: Bag is full!");
                return false;
            }

            Debug.Log($"<color=#FFAA00>[EquipmentSystem]</color> Unequipped <b>{itemToUnequip.itemName}</b>.");
            RecalculateEquipmentBonuses();
            OnEquipmentChanged?.Invoke();
            return true;
        }

        public void DirectEquip(ItemData item)
        {
            if (item == null) return;
            AcquireReferences();

            switch (item.equipmentSlot)
            {
                case EquipmentSlotType.Weapon:
                    _equippedWeapon = item;
                    break;
                case EquipmentSlotType.Armor:
                    _equippedArmor = item;
                    break;
                case EquipmentSlotType.Accessory:
                    _equippedAccessory = item;
                    break;
            }

            RecalculateEquipmentBonuses();
            OnEquipmentChanged?.Invoke();
        }

        public void RecalculateEquipmentBonuses()
        {
            AcquireReferences();

            // 1. Evaluate Weapon Affinity
            HasWeaponAffinity = false;
            if (_equippedWeapon != null && _professionSystem != null && _professionSystem.HasProfession)
            {
                string favored = _professionSystem.CurrentProfession.weaponAffinity;
                if (!string.IsNullOrEmpty(favored) && !string.IsNullOrEmpty(_equippedWeapon.weaponType))
                {
                    if (string.Equals(favored, _equippedWeapon.weaponType, StringComparison.OrdinalIgnoreCase))
                    {
                        HasWeaponAffinity = true;
                        Debug.Log($"<color=#FFD700>★ WEAPON AFFINITY ACTIVE! ★</color> [{_equippedWeapon.weaponType}] perfectly aligns with [{_professionSystem.ProfessionName}]! (+20% Synergy Boost)");
                    }
                }
            }

            // 2. Aggregate equipment bonuses
            float totalBonusAtk = 0f;
            float totalBonusDef = 0f;
            float totalBonusHP = 0f;
            float totalBonusMP = 0f;
            float totalBonusVit = 0f;
            float totalBonusInt = 0f;
            float totalBonusSpd = 0f;

            ItemData[] allGear = { _equippedWeapon, _equippedArmor, _equippedAccessory };
            foreach (var item in allGear)
            {
                if (item == null) continue;
                totalBonusAtk += item.bonusAttack;
                totalBonusDef += item.bonusDefense;
                totalBonusHP += item.bonusMaxHealth;
                totalBonusMP += item.bonusMaxMana;
                totalBonusVit += item.bonusVitality;
                totalBonusInt += item.bonusIntelligence;
                totalBonusSpd += item.bonusSpeed;
            }

            // Apply Affinity Synergy boost to weapon attack
            if (HasWeaponAffinity && _equippedWeapon != null)
            {
                totalBonusAtk += (_equippedWeapon.bonusAttack * 0.20f) + 5.0f;
            }

            // 3. Update PlayerStats
            if (_playerStats != null)
            {
                // Profession modifiers + Equipment modifiers
                float profAtk = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusAttack : 0f;
                float profDef = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusDefense : 0f;
                float profHP = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusMaxHealth : 0f;
                float profMP = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusMaxMana : 0f;
                float profVit = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusVitality : 0f;
                float profInt = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusIntelligence : 0f;
                float profSpd = _professionSystem != null && _professionSystem.HasProfession ? _professionSystem.CurrentProfession.bonusSpeed : 0f;

                _playerStats.BonusAttack = profAtk + totalBonusAtk;
                _playerStats.BonusDefense = profDef + totalBonusDef;
                _playerStats.BonusMaxHealth = profHP + totalBonusHP;
                _playerStats.BonusMaxMana = profMP + totalBonusMP;
                _playerStats.BonusVitality = profVit + totalBonusVit;
                _playerStats.BonusIntelligence = profInt + totalBonusInt;
                _playerStats.BonusSpeed = profSpd + totalBonusSpd;

                _playerStats.RecalculateStats(false);
            }
        }
    }
}
