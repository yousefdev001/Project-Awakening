using Awakening.Core;
using Awakening.Monsters;
using Awakening.Player;
using Awakening.Professions;
using Awakening.Quests;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Victory Credits & Endgame Summary Screen presented after conquering Gorgar the Goblin Chief.
    /// </summary>
    public class VictoryCreditsUI : MonoBehaviour
    {
        public static VictoryCreditsUI Instance { get; private set; }

        private bool _showCredits = false;

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
            if (GoblinChiefBoss.Instance != null)
            {
                GoblinChiefBoss.Instance.OnBossDefeated += HandleBossDefeated;
            }
        }

        private void HandleBossDefeated()
        {
            // Delay 3 seconds after victory banner to show full credits
            Invoke(nameof(ShowVictoryCredits), 3.5f);
        }

        public void ShowVictoryCredits()
        {
            _showCredits = true;
        }

        public void CloseCredits()
        {
            _showCredits = false;
        }

        private void OnGUI()
        {
            if (!_showCredits) return;

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Semi-transparent Golden Obsidian backdrop
            GUI.color = new Color(0.04f, 0.05f, 0.08f, 0.92f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int boxW = 460;
            int boxH = 360;
            int boxX = (screenW - boxW) / 2;
            int boxY = (screenH - boxH) / 2;

            // Main Frame
            GUI.color = new Color(0.08f, 0.1f, 0.15f, 0.98f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // Double Gold Trim
            GUI.color = new Color(1f, 0.85f, 0.2f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 3), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(boxX, boxY + boxH - 3, boxW, 3), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Victory Title
            GUI.Label(new Rect(boxX, boxY + 18, boxW, 32), "<size=18><b><color=#FFD700>👑 THE REALM IS AWAKENED! 👑</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(boxX, boxY + 50, boxW, 20), "<size=10><i><color=#00FFAA>Gorgar has fallen. The Whispering Forest is free of terror.</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // Summary Stats Card
            int cardX = boxX + 25;
            int cardY = boxY + 80;
            int cardW = boxW - 50;
            int cardH = 180;

            GUI.color = new Color(0.05f, 0.07f, 0.1f, 0.8f);
            GUI.DrawTexture(new Rect(cardX, cardY, cardW, cardH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int lineY = cardY + 12;
            int lineHeight = 26;

            int level = PlayerStats.Instance != null ? PlayerStats.Instance.CurrentLevel : 1;
            int gold = PlayerWallet.Instance != null ? PlayerWallet.Instance.CurrentGold : 0;
            string profName = ProfessionSystem.Instance != null && ProfessionSystem.Instance.CurrentProfession != null ?
                $"{ProfessionSystem.Instance.CurrentProfession.professionName} (Rank {ProfessionSystem.Instance.CurrentProfession.rank})" : "Swordsman";
            int questsDone = QuestManager.Instance != null ? QuestManager.Instance.CompletedQuests.Count : 0;

            GUI.Label(new Rect(cardX + 15, lineY, cardW - 30, 20), $"• Awakened Profession: <b><color=#00D4FF>{profName}</color></b>");
            lineY += lineHeight;
            GUI.Label(new Rect(cardX + 15, lineY, cardW - 30, 20), $"• Hero Level Reached: <b><color=#FFD700>Level {level}</color></b>");
            lineY += lineHeight;
            GUI.Label(new Rect(cardX + 15, lineY, cardW - 30, 20), $"• Treasury Wealth Accumulated: <b><color=#FFD700>{gold} Gold</color></b>");
            lineY += lineHeight;
            GUI.Label(new Rect(cardX + 15, lineY, cardW - 30, 20), $"• Quests Completed: <b><color=#00FFAA>{questsDone} Quests</color></b>");
            lineY += lineHeight + 5;
            GUI.Label(new Rect(cardX + 15, lineY, cardW - 30, 24), "<i><color=#DDD>★ Thank you for playing the Project Awakening MVP! ★</color></i>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // Action Buttons
            int btnW = 180;
            int btnH = 34;
            int btnY = boxY + boxH - 55;

            if (GUI.Button(new Rect(boxX + 35, btnY, btnW, btnH), "<b>⚔️ Continue Exploring</b>"))
            {
                CloseCredits();
            }

            if (GUI.Button(new Rect(boxX + boxW - btnW - 35, btnY, btnW, btnH), "<b>🏠 Main Menu</b>"))
            {
                CloseCredits();
                if (GameStateManager.Instance != null) GameStateManager.Instance.SetState(GameState.MainMenu);
            }
        }
    }
}
