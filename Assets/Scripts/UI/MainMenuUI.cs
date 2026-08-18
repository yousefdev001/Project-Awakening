using Awakening.Audio;
using Awakening.Core;
using Awakening.Persistence;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Cinematic Main Menu UI Screen.
    /// Handles Starting New Journeys, Continuing Saved Games, and Audio Configurations.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance { get; private set; }

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
            if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.MainMenu)
            {
                return;
            }

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Full screen dark backdrop
            GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.95f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Game Logo & Title
            int titleY = screenH / 5;
            GUI.Label(new Rect(0, titleY, screenW, 45), "<size=26><b><color=#FFD700>PROJECT: AWAKENING</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(0, titleY + 45, screenW, 25), "<size=12><i><color=#00D4FF>A 3D Action RPG of Celestial Awakenings & Monster Purges</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            int menuW = 280;
            int menuH = 260;
            int menuX = (screenW - menuW) / 2;
            int menuY = titleY + 95;

            if (!_showSettings)
            {
                int btnH = 38;
                int gap = 10;
                int curY = menuY;

                // Start Game Button
                if (GUI.Button(new Rect(menuX, curY, menuW, btnH), "<b>▶ Start New Journey</b>"))
                {
                    AudioManager.Instance?.PlaySound(SoundType.InteractClick);
                    GameStateManager.Instance.SetState(GameState.Gameplay);
                }
                curY += btnH + gap;

                // Continue Game Button
                bool hasSave = SaveSystem.HasSaveFile;
                GUI.enabled = hasSave;
                string continueText = hasSave ? "<b>📂 Continue Saved Game</b>" : "📂 Continue (No Save)";
                if (GUI.Button(new Rect(menuX, curY, menuW, btnH), continueText))
                {
                    AudioManager.Instance?.PlaySound(SoundType.InteractClick);
                    if (SaveSystem.Instance != null) SaveSystem.Instance.LoadGame();
                    GameStateManager.Instance.SetState(GameState.Gameplay);
                }
                GUI.enabled = true;
                curY += btnH + gap;

                // Settings Button
                if (GUI.Button(new Rect(menuX, curY, menuW, btnH), "<b>⚙️ Audio Settings</b>"))
                {
                    AudioManager.Instance?.PlaySound(SoundType.InteractClick);
                    _showSettings = true;
                }
                curY += btnH + gap;

                // Quit Game Button
                if (GUI.Button(new Rect(menuX, curY, menuW, btnH), "<b>✕ Quit Game</b>"))
                {
                    Application.Quit();
                }
            }
            else
            {
                // Audio Settings Sub-Menu
                GUI.Box(new Rect(menuX, menuY, menuW, 200), "⚙️ Audio Configurations");

                AudioManager am = AudioManager.Instance;
                if (am != null)
                {
                    GUI.Label(new Rect(menuX + 15, menuY + 35, 100, 20), $"Master: {(am.MasterVolume * 100):F0}%");
                    am.MasterVolume = GUI.HorizontalSlider(new Rect(menuX + 110, menuY + 40, menuW - 125, 15), am.MasterVolume, 0f, 1f);

                    GUI.Label(new Rect(menuX + 15, menuY + 70, 100, 20), $"SFX: {(am.SFXVolume * 100):F0}%");
                    am.SFXVolume = GUI.HorizontalSlider(new Rect(menuX + 110, menuY + 75, menuW - 125, 15), am.SFXVolume, 0f, 1f);

                    GUI.Label(new Rect(menuX + 15, menuY + 105, 100, 20), $"BGM: {(am.BGMVolume * 100):F0}%");
                    am.BGMVolume = GUI.HorizontalSlider(new Rect(menuX + 110, menuY + 110, menuW - 125, 15), am.BGMVolume, 0f, 1f);
                }

                if (GUI.Button(new Rect(menuX + 20, menuY + 150, menuW - 40, 32), "✔ Back to Menu"))
                {
                    _showSettings = false;
                }
            }
        }
    }
}
