using UnityEngine;

namespace Awakening.Items
{
    /// <summary>
    /// ScriptableObject defining an Item's properties, value, and effects.
    /// Data-Driven: Adding new items requires only creating a new asset.
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

        #region MVP Items Presets Factory
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

        public static ItemData CreateGoblinDaggerPreset()
        {
            var data = ScriptableObject.CreateInstance<ItemData>();
            data.itemID = "WEAP_GOBLIN_DAGGER";
            data.itemName = "Goblin Scrap Dagger";
            data.itemType = ItemType.Weapon;
            data.rarity = ItemRarity.Uncommon;
            data.description = "A jagged, crude iron blade salvaged from a goblin warrior.";
            data.themeColor = new Color(0.85f, 0.4f, 0.2f);
            data.goldValue = 35;
            data.isStackable = false;
            data.maxStackSize = 1;
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
        #endregion
    }
}
