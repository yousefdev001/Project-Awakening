using System;
using System.Collections.Generic;

namespace Awakening.Persistence
{
    /// <summary>
    /// Root serializable container for all saved player, RPG progression, inventory, and world states.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public string saveTimestamp;
        public int saveVersion = 1;

        // Player & Stats
        public PlayerStatsSaveData stats = new PlayerStatsSaveData();

        // Awakening & Profession
        public ProfessionSaveData profession = new ProfessionSaveData();

        // Inventory & Equipment
        public List<ItemSlotSaveData> inventoryItems = new List<ItemSlotSaveData>();
        public EquipmentSaveData equipment = new EquipmentSaveData();

        // Quests
        public List<QuestSaveData> quests = new List<QuestSaveData>();

        // World Progress
        public bool bossDefeated = false;
    }

    [Serializable]
    public class PlayerStatsSaveData
    {
        public int level = 1;
        public float currentXP = 0f;
        public float requiredXP = 100f;
        public int gold = 100;
        public float currentHealth = 100f;
        public float currentMana = 50f;
    }

    [Serializable]
    public class ProfessionSaveData
    {
        public string professionID = "WARRIOR";
        public string professionName = "Swordsman";
        public string rank = "C";
        public string weaponAffinity = "Sword";
        public bool hasAwakened = true;
    }

    [Serializable]
    public class ItemSlotSaveData
    {
        public string itemID;
        public string itemName;
        public int quantity;
        public int slotIndex;
        public string itemType;
    }

    [Serializable]
    public class EquipmentSaveData
    {
        public string weaponID = "";
        public string armorID = "";
        public string accessoryID = "";
    }

    [Serializable]
    public class QuestSaveData
    {
        public string questID;
        public int currentAmount;
        public string state;
    }
}
