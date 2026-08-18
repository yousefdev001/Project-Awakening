using Awakening.Quests;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// HUD overlay displaying active quest objectives, live kill progress, and turn-in notifications.
    /// </summary>
    public class QuestTrackerHUD : MonoBehaviour
    {
        [SerializeField] private bool _showHUD = true;

        private void OnGUI()
        {
            if (!_showHUD) return;

            QuestManager qm = QuestManager.Instance;
            if (qm == null || qm.ActiveQuests.Count == 0) return;

            int boxW = 230;
            int boxH = 40 + (qm.ActiveQuests.Count * 75);
            int boxX = Screen.width - 240;
            int boxY = 10;

            // Semi-transparent background
            GUI.color = new Color(0.04f, 0.07f, 0.12f, 0.85f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // Gold header line
            GUI.color = new Color(1f, 0.84f, 0.2f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(boxX + 10, boxY + 8, boxW - 20, 20), "<b>📜 ACTIVE QUESTS</b>", new GUIStyle(GUI.skin.label) { fontSize = 11 });

            int itemY = boxY + 30;

            for (int i = 0; i < qm.ActiveQuests.Count; i++)
            {
                QuestData quest = qm.ActiveQuests[i];

                GUI.Label(new Rect(boxX + 10, itemY, boxW - 20, 18), $"<size=10><b><color=yellow>{quest.questTitle}</color></b></size>");

                if (quest.state == QuestState.CanTurnIn || quest.IsComplete)
                {
                    GUI.Label(new Rect(boxX + 10, itemY + 16, boxW - 20, 18), "<size=9><b><color=#00FFAA>★ OBJECTIVE COMPLETE! ★</color></b></size>");
                    if (GUI.Button(new Rect(boxX + 10, itemY + 36, boxW - 20, 24), "🏆 Turn In Quest (Claim)"))
                    {
                        qm.TurnInQuest(quest.questID);
                    }
                }
                else
                {
                    GUI.Label(new Rect(boxX + 10, itemY + 16, boxW - 20, 18), $"<size=9>• Slay {quest.targetName}: <b>{quest.currentAmount}/{quest.targetAmount}</b></size>");

                    // Small Progress Bar
                    int barW = boxW - 20;
                    int barH = 10;
                    GUI.Box(new Rect(boxX + 10, itemY + 36, barW, barH), "");

                    float progress = quest.targetAmount > 0 ? (float)quest.currentAmount / quest.targetAmount : 0f;
                    GUI.color = new Color(0.2f, 0.8f, 1.0f);
                    GUI.DrawTexture(new Rect(boxX + 11, itemY + 37, (barW - 2) * progress, barH - 2), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                itemY += 75;
            }
        }
    }
}
