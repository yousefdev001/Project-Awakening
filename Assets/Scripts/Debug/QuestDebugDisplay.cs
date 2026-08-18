using Awakening.Quests;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to test Quest acceptance, kill simulations, and reward turn-ins.
    /// </summary>
    public class QuestDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            QuestManager qm = QuestManager.Instance;
            if (qm == null) return;

            int boxW = 260;
            int boxH = 145;
            int boxX = 10;
            int boxY = 570;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "📜 Quest System Journal (Phase 20)");

            GUI.Label(new Rect(boxX + 10, boxY + 25, boxW - 20, 20), $"Active Quests: <b>{qm.ActiveQuests.Count}</b> | Completed: <b>{qm.CompletedQuests.Count}</b>");

            int btnY = boxY + 48;
            if (GUI.Button(new Rect(boxX + 10, btnY, 115, 24), "+Wolves Quest"))
            {
                qm.AcceptQuest(QuestData.CreateForestWolvesQuestPreset());
            }

            if (GUI.Button(new Rect(boxX + 135, btnY, 115, 24), "+Goblins Quest"))
            {
                qm.AcceptQuest(QuestData.CreateGoblinIncursionQuestPreset());
            }

            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 115, 24), "⚔️ Kill Wolf (Test)"))
            {
                qm.RecordMonsterKill("MON_WOLF", 1);
            }

            if (GUI.Button(new Rect(boxX + 135, btnY2, 115, 24), "⚔️ Kill Goblin (Test)"))
            {
                qm.RecordMonsterKill("MON_GOBLIN", 1);
            }

            int btnY3 = btnY2 + 26;
            if (GUI.Button(new Rect(boxX + 10, btnY3, boxW - 20, 22), "🏆 Auto-Turn In All Completed Quests"))
            {
                for (int i = qm.ActiveQuests.Count - 1; i >= 0; i--)
                {
                    var q = qm.ActiveQuests[i];
                    if (q.IsComplete || q.state == QuestState.CanTurnIn)
                    {
                        qm.TurnInQuest(q.questID);
                    }
                }
            }
        }
    }
}
