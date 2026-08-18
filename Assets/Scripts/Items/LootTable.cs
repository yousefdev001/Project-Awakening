using System;
using System.Collections.Generic;
using UnityEngine;

namespace Awakening.Items
{
    [Serializable]
    public class LootDropEntry
    {
        public ItemData item;
        [Range(0f, 100f)] public float dropChance = 50f;
        public int minQuantity = 1;
        public int maxQuantity = 1;

        public LootDropEntry(ItemData itemData, float chance, int minQty, int maxQty)
        {
            item = itemData;
            dropChance = chance;
            minQuantity = minQty;
            maxQuantity = maxQty;
        }
    }

    public struct DroppedItemResult
    {
        public ItemData Item;
        public int Quantity;

        public DroppedItemResult(ItemData item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// ScriptableObject defining the random drop table of a monster or chest.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Awakening/Items/Loot Table")]
    public class LootTable : ScriptableObject
    {
        public List<LootDropEntry> drops = new List<LootDropEntry>();

        public List<DroppedItemResult> RollDrops()
        {
            List<DroppedItemResult> results = new List<DroppedItemResult>();

            foreach (var entry in drops)
            {
                if (entry.item == null) continue;

                float roll = UnityEngine.Random.Range(0f, 100f);
                if (roll <= entry.dropChance)
                {
                    int qty = UnityEngine.Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                    results.Add(new DroppedItemResult(entry.item, qty));
                }
            }

            return results;
        }

        #region MVP Preset Tables
        public static LootTable CreateSlimeLootTable()
        {
            var table = ScriptableObject.CreateInstance<LootTable>();
            table.drops.Add(new LootDropEntry(ItemData.CreateGoldPreset(5), 100f, 1, 1));     // 100% Gold
            table.drops.Add(new LootDropEntry(ItemData.CreateSlimeJellyPreset(), 80f, 1, 2)); // 80% Jelly
            table.drops.Add(new LootDropEntry(ItemData.CreateHealthPotionPreset(), 25f, 1, 1)); // 25% Potion
            return table;
        }

        public static LootTable CreateWolfLootTable()
        {
            var table = ScriptableObject.CreateInstance<LootTable>();
            table.drops.Add(new LootDropEntry(ItemData.CreateGoldPreset(15), 100f, 1, 1));   // 100% Gold
            table.drops.Add(new LootDropEntry(ItemData.CreateWolfFurPreset(), 75f, 1, 2));   // 75% Fur
            table.drops.Add(new LootDropEntry(ItemData.CreateWolfFangPreset(), 40f, 1, 1));  // 40% Fang
            return table;
        }

        public static LootTable CreateGoblinLootTable()
        {
            var table = ScriptableObject.CreateInstance<LootTable>();
            table.drops.Add(new LootDropEntry(ItemData.CreateGoldPreset(25), 100f, 1, 1));       // 100% Gold
            table.drops.Add(new LootDropEntry(ItemData.CreateGoblinDaggerPreset(), 35f, 1, 1));  // 35% Dagger
            table.drops.Add(new LootDropEntry(ItemData.CreateManaPotionPreset(), 50f, 1, 1));    // 50% Mana Potion
            return table;
        }
        #endregion
    }
}
