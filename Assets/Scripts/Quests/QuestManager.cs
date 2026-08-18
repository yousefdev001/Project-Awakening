using System;
using System.Collections.Generic;
using Awakening.Inventory;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Quests
{
    /// <summary>
    /// Core Player Quest Manager maintaining active/completed quests, tracking monster kills, and awarding rewards.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Starting Quest")]
        [SerializeField] private bool _autoAcceptStartingQuest = true;

        public IReadOnlyList<QuestData> ActiveQuests => _activeQuests;
        public IReadOnlyList<QuestData> CompletedQuests => _completedQuests;

        public event Action<QuestData> OnQuestAccepted;
        public event Action<QuestData> OnQuestProgressUpdated;
        public event Action<QuestData> OnQuestCompleted;
        public event Action OnQuestListChanged;

        private List<QuestData> _activeQuests = new List<QuestData>();
        private List<QuestData> _completedQuests = new List<QuestData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (_autoAcceptStartingQuest && _activeQuests.Count == 0)
            {
                AcceptQuest(QuestData.CreateForestWolvesQuestPreset());
            }
        }

        public bool AcceptQuest(QuestData questTemplate)
        {
            if (questTemplate == null) return false;

            // Check if already active or completed
            if (_activeQuests.Exists(q => q.questID == questTemplate.questID) ||
                _completedQuests.Exists(q => q.questID == questTemplate.questID))
            {
                Debug.LogWarning($"[QuestManager] Quest '{questTemplate.questTitle}' is already active or finished.");
                return false;
            }

            QuestData runtimeQuest = questTemplate.CreateRuntimeClone();
            runtimeQuest.state = QuestState.InProgress;
            _activeQuests.Add(runtimeQuest);

            Debug.Log($"<color=#FFD700>📜 [Quest Accepted]</color> <b>{runtimeQuest.questTitle}</b> — Slay {runtimeQuest.targetAmount}x {runtimeQuest.targetName}.");

            OnQuestAccepted?.Invoke(runtimeQuest);
            OnQuestListChanged?.Invoke();
            return true;
        }

        public void RecordMonsterKill(string monsterID, int amount = 1)
        {
            if (string.IsNullOrEmpty(monsterID)) return;

            bool anyUpdated = false;

            foreach (var quest in _activeQuests)
            {
                if (quest.state == QuestState.InProgress && quest.questType == QuestType.KillMonsters)
                {
                    if (quest.targetID == monsterID || monsterID.Contains(quest.targetID) || quest.targetID.Contains(monsterID))
                    {
                        quest.currentAmount = Mathf.Min(quest.targetAmount, quest.currentAmount + amount);
                        anyUpdated = true;

                        Debug.Log($"<color=#00FFAA>⚔️ [Quest Progress]</color> <b>{quest.questTitle}</b>: {quest.currentAmount}/{quest.targetAmount} {quest.targetName}");

                        if (quest.currentAmount >= quest.targetAmount)
                        {
                            quest.state = QuestState.CanTurnIn;
                            Debug.Log($"<color=#FFFF00>★ [Quest Objective Complete!] ★</color> <b>{quest.questTitle}</b> is ready to turn in!");
                        }

                        OnQuestProgressUpdated?.Invoke(quest);
                    }
                }
            }

            if (anyUpdated)
            {
                OnQuestListChanged?.Invoke();
            }
        }

        public bool TurnInQuest(string questID)
        {
            QuestData quest = _activeQuests.Find(q => q.questID == questID);
            if (quest == null) return false;

            if (quest.state != QuestState.CanTurnIn && !quest.IsComplete)
            {
                Debug.LogWarning($"[QuestManager] Quest '{quest.questTitle}' is not complete yet!");
                return false;
            }

            quest.state = QuestState.Completed;
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);

            Debug.Log($"<color=#FFD700>🏆 [Quest Completed!]</color> <b>{quest.questTitle}</b>! Granting rewards...");

            // 1. Award XP
            if (PlayerProgression.Instance != null && quest.rewardXP > 0f)
            {
                PlayerProgression.Instance.AddXP(quest.rewardXP);
            }

            // 2. Award Gold
            if (PlayerWallet.Instance != null && quest.rewardGold > 0)
            {
                PlayerWallet.Instance.AddGold(quest.rewardGold);
            }

            // 3. Award Item
            if (InventorySystem.Instance != null && quest.rewardItem != null)
            {
                InventorySystem.Instance.AddItem(quest.rewardItem, 1);
            }

            OnQuestCompleted?.Invoke(quest);
            OnQuestListChanged?.Invoke();
            return true;
        }
    }
}
