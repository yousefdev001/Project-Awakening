using System;
using System.IO;
using Awakening.Equipment;
using Awakening.Inventory;
using Awakening.Items;
using Awakening.Monsters;
using Awakening.Player;
using Awakening.Professions;
using Awakening.Quests;
using UnityEngine;

namespace Awakening.Persistence
{
    /// <summary>
    /// Master Save and Load Persistence Engine.
    /// Serializes complete player RPG state, items, gold, equipment, and quests to persistent JSON disk storage.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        public static string SaveFilePath => Path.Combine(Application.persistentDataPath, "Awakening_Save.json");
        public static bool HasSaveFile => File.Exists(SaveFilePath);

        public event Action<SaveData> OnGameSaved;
        public event Action<SaveData> OnGameLoaded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool SaveGame()
        {
            try
            {
                SaveData data = new SaveData();
                data.saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 1. Save Stats & Progression
                if (PlayerStats.Instance != null)
                {
                    data.stats.level = PlayerStats.Instance.CurrentLevel;
                    data.stats.currentHealth = PlayerStats.Instance.CurrentHealth;
                    data.stats.currentMana = PlayerStats.Instance.CurrentMana;
                }

                if (PlayerProgression.Instance != null)
                {
                    data.stats.currentXP = PlayerProgression.Instance.CurrentXP;
                    data.stats.requiredXP = PlayerProgression.Instance.RequiredXP;
                }

                if (PlayerWallet.Instance != null)
                {
                    data.stats.gold = PlayerWallet.Instance.CurrentGold;
                }

                // 2. Save Profession & Awakening
                if (ProfessionSystem.Instance != null && ProfessionSystem.Instance.CurrentProfession != null)
                {
                    var prof = ProfessionSystem.Instance.CurrentProfession;
                    data.profession.professionID = prof.professionID;
                    data.profession.professionName = prof.professionName;
                    data.profession.rank = prof.rank.ToString();
                    data.profession.weaponAffinity = prof.weaponAffinity;
                    data.profession.hasAwakened = ProfessionSystem.Instance.HasAwakened;
                }

                // 3. Save Inventory Items
                if (InventorySystem.Instance != null)
                {
                    for (int i = 0; i < InventorySystem.Instance.Slots.Count; i++)
                    {
                        var slot = InventorySystem.Instance.Slots[i];
                        if (!slot.IsEmpty && slot.Item != null)
                        {
                            data.inventoryItems.Add(new ItemSlotSaveData
                            {
                                itemID = slot.Item.itemID,
                                itemName = slot.Item.itemName,
                                quantity = slot.Quantity,
                                slotIndex = i,
                                itemType = slot.Item.itemType.ToString()
                            });
                        }
                    }
                }

                // 4. Save Quests
                if (QuestManager.Instance != null)
                {
                    foreach (var quest in QuestManager.Instance.ActiveQuests)
                    {
                        data.quests.Add(new QuestSaveData
                        {
                            questID = quest.questID,
                            currentAmount = quest.currentAmount,
                            state = quest.state.ToString()
                        });
                    }
                }

                // 5. Save World Boss State
                if (GoblinChiefBoss.Instance != null)
                {
                    data.bossDefeated = GoblinChiefBoss.Instance.IsDefeated;
                }

                // Write to JSON file
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);

                Debug.Log($"<color=#00FFAA>💾 [SaveSystem]</color> Game successfully saved to: <b>{SaveFilePath}</b>");
                OnGameSaved?.Invoke(data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to save game: {ex.Message}");
                return false;
            }
        }

        public bool LoadGame()
        {
            if (!HasSaveFile)
            {
                Debug.LogWarning("[SaveSystem] No save file found to load.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data == null) return false;

                // 1. Restore Stats & Wallet
                if (PlayerWallet.Instance != null)
                {
                    int currentGold = PlayerWallet.Instance.CurrentGold;
                    int diff = data.stats.gold - currentGold;
                    if (diff > 0) PlayerWallet.Instance.AddGold(diff);
                    else if (diff < 0) PlayerWallet.Instance.SpendGold(-diff);
                }

                if (PlayerProgression.Instance != null)
                {
                    PlayerProgression.Instance.ResetProgression();
                    if (data.stats.currentXP > 0f)
                    {
                        PlayerProgression.Instance.AddXP(data.stats.currentXP);
                    }
                }

                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.Heal(data.stats.currentHealth);
                    PlayerStats.Instance.RestoreMana(data.stats.currentMana);
                }

                // 2. Restore Inventory Items (Clear and repopulate)
                if (InventorySystem.Instance != null && data.inventoryItems != null)
                {
                    InventorySystem.Instance.ClearInventory();
                    foreach (var itemSave in data.inventoryItems)
                    {
                        ItemData item = ItemData.CreatePresetByID(itemSave.itemID);
                        if (item != null)
                        {
                            InventorySystem.Instance.AddItem(item, itemSave.quantity);
                        }
                    }
                }

                Debug.Log($"<color=#00D4FF>📂 [SaveSystem]</color> Game loaded successfully! (Saved at: {data.saveTimestamp})");
                OnGameLoaded?.Invoke(data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to load game: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSaveFile()
        {
            if (HasSaveFile)
            {
                File.Delete(SaveFilePath);
                Debug.Log("<color=#FFAA00>🗑️ [SaveSystem]</color> Save file deleted.");
                return true;
            }
            return false;
        }
    }
}
