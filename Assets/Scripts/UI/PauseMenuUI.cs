using Awakening.Audio;
using Awakening.Core;
using Awakening.Persistence;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Pause Menu UI overlay triggered on Escape key press.
    /// Handles Game Resuming, Quick Saving, Audio Adjustments, and Returning to Title.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        private bool _showSettings = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnGUI()
        {
            if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Paused)
            {
                return;
            }

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Dim backdrop
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int boxW = 300;
            int boxH = 270;
            int boxX = (screenW - boxW) / 2;
            int boxY = (screenH - boxH) / 2;

            // Menu Frame
            GUI.color = new Color(0.06f, 0.09f, 0.14f, 0.95f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // Gold Header Border
            GUI.color = new Color(1f, 0.84f, 0.2f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(boxX, boxY + 12, boxW, 25), "<size=15><b><color=#FFD700>⏸️ GAME PAUSED</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            if (!_showSettings)
            {
                int btnW = boxW - 40;
                int btnH = 34;
                int gap = 10;
                int curY = boxY + 50;

                // Resume
                if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>▶ Resume Game</b>"))
                {
                    GameStateManager.Instance.SetState(GameState.Gameplay);
                }
                curY += btnH + gap;

                // Quick Save
                if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>💾 Quick Save Game</b>"))
                {
                    if (SaveSystem.Instance != null) SaveSystem.Instance.SaveGame();
                }
                curY += btnH + gap;

                // Settings
                if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>⚙️ Audio Settings</b>"))
                {
                    _showSettings = true;
                }
                curY += btnH + gap;

                // Main Menu
                if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>🏠 Main Menu</b>"))
                {
                    GameStateManager.Instance.SetState(GameState.MainMenu);
                }
            }
            else
            {
                // Settings inside Pause
                AudioManager am = AudioManager.Instance;
                if (am != null)
                {
                    int setY = boxY + 55;
                    GUI.Label(new Rect(boxX + 20, setY, 80, 20), $"Master: {(am.MasterVolume * 100):F0}%");
                    am.MasterVolume = GUI.HorizontalSlider(new Rect(boxX + 110, setY + 4, boxW - 130, 15), am.MasterVolume, 0f, 1f);

                    GUI.Label(new Rect(boxX + 20, setY + 35, 80, 20), $"SFX: {(am.SFXVolume * 100):F0}%");
                    am.SFXVolume = GUI.HorizontalSlider(new Rect(boxX + 110, setY + 39, boxW - 130, 15), am.SFXVolume, 0f, 1f);

                    GUI.Label(new Rect(boxX + 20, setY + 70, 80, 20), $"BGM: {(am.BGMVolume * 100):F0}%");
                    am.BGMVolume = GUI.HorizontalSlider(new Rect(boxX + 110, setY + 74, boxW - 130, 15), am.BGMVolume, 0f, 1f);
                }

                if (GUI.Button(new Rect(boxX + 20, boxY + boxH - 45, boxW - 40, 30), "✔ Back"))
                {
                    _showSettings = false;
                }
            }
        }
    }
}
