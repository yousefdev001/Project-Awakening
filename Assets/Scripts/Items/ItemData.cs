using Awakening.Equipment;
using UnityEngine;

namespace Awakening.Items
{
    /// <summary>
    /// ScriptableObject defining an Item's properties, economy, and equipment stats.
    /// Data-Driven: Adding new items, weapons, and armors requires zero C# script modifications.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "Awakening/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemID = "ITEM_GOLD";
        public string itemName = "Gold Coins";
        public ItemType itemType = ItemType.Gold;
        public ItemRarity rarity = ItemRarity.Common;

        [TextArea(2, 4)]
        public string description = "Standard currency accepted across all kingdoms and villages.";

        public Color themeColor = new Color(1f, 0.85f, 0.2f); // Gold yellow

        [Header("Economy & Stacking")]
        public int goldValue = 1;
        public bool isStackable = true;
        public int maxStackSize = 99;

        [Header("Consumable Effects (If Applicable)")]
        public float restoreHealthAmount = 0f;
        public float restoreManaAmount = 0f;

        [Header("Equipment Attributes (If Applicable)")]
        public EquipmentSlotType equipmentSlot = EquipmentSlotType.None;
        public string weaponType = ""; // "Sword", "Bow", "Staff", "Dagger"
        public float bonusAttack = 0f;
        public float bonusDefense = 0f;
        public float bonusMaxHealth = 0f;
        public float bonusMaxMana = 0f;
        public float bonusVitality = 0f;
        public float bonusIntelligence = 0f;
        public float bonusSpeed = 0f;

        #region MVP Items & Equipment Presets Factory
        public static ItemData CreateGoldPreset(int amount = 10)
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "ITEM_GOLD";
            data.itemName = "Gold Coins";
            data.itemType = ItemType.Gold;
            data.rarity = ItemRarity.Common;
            data.description = "Precious golden coins minted in the kingdom.";
            data.themeColor = new Color(1f, 0.84f, 0.0f);
            data.goldValue = amount;
            data.isStackable = true;
            data.maxStackSize = 99999;
            return data;
        }

        public static ItemData CreateSlimeJellyPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "MAT_SLIME_JELLY";
            data.itemName = "Slime Jelly";
            data.itemType = ItemType.Material;
            data.rarity = ItemRarity.Common;
            data.description = "Viscous green residue harvested from defeated slimes. Used in crafting.";
            data.themeColor = new Color(0.2f, 0.9f, 0.3f);
            data.goldValue = 5;
            data.isStackable = true;
            data.maxStackSize = 50;
            return data;
        }

        public static ItemData CreateWolfFurPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "MAT_WOLF_FUR";
            data.itemName = "Wolf Fur";
            data.itemType = ItemType.Material;
            data.rarity = ItemRarity.Uncommon;
            data.description = "Thick, warm pelt of a wild forest wolf. Highly valued by village tailors.";
            data.themeColor = new Color(0.6f, 0.6f, 0.7f);
            data.goldValue = 15;
            data.isStackable = true;
            data.maxStackSize = 30;
            return data;
        }

        public static ItemData CreateWolfFangPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "MAT_WOLF_FANG";
            data.itemName = "Wolf Fang";
            data.itemType = ItemType.Material;
            data.rarity = ItemRarity.Rare;
            data.description = "Sharp predatory canine tooth. Useful for smithing daggers and arrows.";
            data.themeColor = new Color(0.95f, 0.95f, 0.85f);
            data.goldValue = 25;
            data.isStackable = true;
            data.maxStackSize = 30;
            return data;
        }

        public static ItemData CreateHealthPotionPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "CONS_HEALTH_POTION";
            data.itemName = "Health Potion";
            data.itemType = ItemType.Consumable;
            data.rarity = ItemRarity.Common;
            data.description = "Restores +50 Health instantly when consumed.";
            data.themeColor = new Color(1f, 0.25f, 0.25f);
            data.goldValue = 20;
            data.isStackable = true;
            data.maxStackSize = 20;
            data.restoreHealthAmount = 50f;
            return data;
        }

        public static ItemData CreateManaPotionPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "CONS_MANA_POTION";
            data.itemName = "Mana Potion";
            data.itemType = ItemType.Consumable;
            data.rarity = ItemRarity.Common;
            data.description = "Restores +50 Mana instantly when consumed.";
            data.themeColor = new Color(0.1f, 0.6f, 1f);
            data.goldValue = 20;
            data.isStackable = true;
            data.maxStackSize = 20;
            data.restoreManaAmount = 50f;
            return data;
        }

        // --- Equipment Presets ---

        public static ItemData CreateIronLongswordPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "WEAP_IRON_LONGSWORD";
            data.itemName = "Iron Longsword";
            data.itemType = ItemType.Weapon;
            data.equipmentSlot = EquipmentSlotType.Weapon;
            data.weaponType = "Sword";
            data.rarity = ItemRarity.Uncommon;
            data.description = "A tempered steel longsword with balanced weight. Favored weapon of Swordsmen (+20% Affinity).";
            data.themeColor = new Color(0.7f, 0.85f, 1.0f);
            data.goldValue = 60;
            data.isStackable = false;
            data.maxStackSize = 1;
            data.bonusAttack = 18f;
            data.bonusDefense = 3f;
            return data;
        }

        public static ItemData CreateHunterBowPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "WEAP_HUNTER_BOW";
            data.itemName = "Hunter Recurve Bow";
            data.itemType = ItemType.Weapon;
            data.equipmentSlot = EquipmentSlotType.Weapon;
            data.weaponType = "Bow";
            data.rarity = ItemRarity.Rare;
            data.description = "A flexible elm recurve bow crafted for rapid marksmanship. Favored by Hunters (+20% Affinity).";
            data.themeColor = new Color(0.3f, 0.9f, 0.6f);
            data.goldValue = 85;
            data.isStackable = false;
            data.maxStackSize = 1;
            data.bonusAttack = 16f;
            data.bonusSpeed = 1.0f;
            return data;
        }

        public static ItemData CreateArcaneStaffPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "WEAP_ARCANE_STAFF";
            data.itemName = "Arcane Oak Staff";
            data.itemType = ItemType.Weapon;
            data.equipmentSlot = EquipmentSlotType.Weapon;
            data.weaponType = "Staff";
            data.rarity = ItemRarity.Epic;
            data.description = "A consecrated staff embedded with an arcane sapphire. Favored by Battle Mages (+25% Affinity).";
            data.themeColor = new Color(0.8f, 0.4f, 1.0f);
            data.goldValue = 120;
            data.isStackable = false;
            data.maxStackSize = 1;
            data.bonusAttack = 14f;
            data.bonusIntelligence = 10f;
            data.bonusMaxMana = 40f;
            return data;
        }

        public static ItemData CreateKnightArmorPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "ARMOR_KNIGHT_PLATE";
            data.itemName = "Iron Vanguard Plate";
            data.itemType = ItemType.Armor;
            data.equipmentSlot = EquipmentSlotType.Armor;
            data.rarity = ItemRarity.Uncommon;
            data.description = "Heavy reinforced steel chestplate offering solid protection.";
            data.themeColor = new Color(0.65f, 0.7f, 0.75f);
            data.goldValue = 75;
            data.isStackable = false;
            data.maxStackSize = 1;
            data.bonusDefense = 14f;
            data.bonusMaxHealth = 50f;
            data.bonusVitality = 4f;
            return data;
        }

        public static ItemData CreateGoblinDaggerPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "WEAP_GOBLIN_DAGGER";
            data.itemName = "Goblin Scrap Dagger";
            data.itemType = ItemType.Weapon;
            data.equipmentSlot = EquipmentSlotType.Weapon;
            data.weaponType = "Dagger";
            data.rarity = ItemRarity.Uncommon;
            data.description = "A jagged, crude iron blade salvaged from a goblin warrior.";
            data.themeColor = new Color(0.85f, 0.4f, 0.2f);
            data.goldValue = 35;
            data.isStackable = false;
            data.maxStackSize = 1;
            data.bonusAttack = 8f;
            data.bonusSpeed = 0.5f;
            return data;
        }

        public static ItemData CreatePresetByID(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            switch (id)
            {
                case "CURR_GOLD": return CreateGoldPreset(20);
                case "MAT_SLIME_JELLY": return CreateSlimeJellyPreset();
                case "MAT_WOLF_FUR": return CreateWolfFurPreset();
                case "MAT_WOLF_FANG": return CreateWolfFangPreset();
                case "CONS_HEALTH_POTION": return CreateHealthPotionPreset();
                case "CONS_MANA_POTION": return CreateManaPotionPreset();
                case "WEAP_IRON_LONGSWORD": return CreateIronLongswordPreset();
                case "WEAP_HUNTER_BOW": return CreateHunterBowPreset();
                case "WEAP_ARCANE_STAFF": return CreateArcaneStaffPreset();
                case "ARMOR_KNIGHT_PLATE": return CreateKnightArmorPreset();
                case "WEAP_GOBLIN_DAGGER": return CreateGoblinDaggerPreset();
                default: return CreateHealthPotionPreset();
            }
        }
        #endregion
    }
}
