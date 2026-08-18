using Awakening.Items;
using UnityEngine;

namespace Awakening.Quests
{
    /// <summary>
    /// ScriptableObject defining a Quest, its objectives, progress, and rewards.
    /// Data-Driven: Adding new story/side quests requires zero C# code modifications.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuestData", menuName = "Awakening/Quests/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [Header("Identity")]
        public string questID = "QUEST_FOREST_WOLVES";
        public string questTitle = "The Whispering Forest";
        [TextArea(2, 4)]
        public string questDescription = "Wild wolves are attacking supply caravans traveling north. Cull their numbers to safeguard the village.";

        [Header("Objective")]
        public QuestType questType = QuestType.KillMonsters;
        public string targetID = "MON_WOLF";
        public string targetName = "Wild Forest Wolves";
        public int targetAmount = 3;

        [Header("Runtime Progress (Instantiated Clones)")]
        public int currentAmount = 0;
        public QuestState state = QuestState.NotStarted;

        [Header("Rewards")]
        public float rewardXP = 150f;
        public int rewardGold = 50;
        public ItemData rewardItem;

        public bool IsComplete => currentAmount >= targetAmount;

        public QuestData CreateRuntimeClone()
        {
            QuestData clone = Instantiate(this);
            clone.currentAmount = 0;
            clone.state = QuestState.NotStarted;
            return clone;
        }

        #region MVP Quests Presets Factory
        public static QuestData CreateForestWolvesQuestPreset()
        {
            var data = ScriptableObject.CreateInstance<QuestData>();
            data.questID = "QUEST_FOREST_WOLVES";
            data.questTitle = "The Whispering Forest";
            data.questDescription = "Wild forest wolves threaten the outer farms. Slay 3 Wild Wolves and return to Elder Eldrin.";
            data.questType = QuestType.KillMonsters;
            data.targetID = "MON_WOLF";
            data.targetName = "Wild Forest Wolves";
            data.targetAmount = 3;
            data.rewardXP = 150f;
            data.rewardGold = 50;
            data.rewardItem = ItemData.CreateIronLongswordPreset();
            data.state = QuestState.NotStarted;
            return data;
        }

        public static QuestData CreateGoblinIncursionQuestPreset()
        {
            var data = ScriptableObject.CreateInstance<QuestData>();
            data.questID = "QUEST_GOBLIN_INCURSION";
            data.questTitle = "Goblin Incursion";
            data.questDescription = "Goblin scouts have established a forward warcamp near the ruins. Defeat 3 Goblin Warriors.";
            data.questType = QuestType.KillMonsters;
            data.targetID = "MON_GOBLIN";
            data.targetName = "Goblin Warriors";
            data.targetAmount = 3;
            data.rewardXP = 250f;
            data.rewardGold = 100;
            data.rewardItem = ItemData.CreateKnightArmorPreset();
            data.state = QuestState.NotStarted;
            return data;
        }
        #endregion
    }
}
