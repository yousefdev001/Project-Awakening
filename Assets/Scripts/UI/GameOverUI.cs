using Awakening.Combat;
using Awakening.Core;
using Awakening.Persistence;
using Awakening.Player;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Game Over UI screen displayed upon player death.
    /// Handles respawning at the safe Village Campfire or loading the last persistent save.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance { get; private set; }

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
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                HealthSystem hs = player.GetComponent<HealthSystem>();
                if (hs != null)
                {
                    hs.OnDeath += HandlePlayerDeath;
                }
            }
        }

        private void HandlePlayerDeath()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.GameOver);
            }
        }

        private void OnGUI()
        {
            if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.GameOver)
            {
                return;
            }

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Crimson Dark Vignette Backdrop
            GUI.color = new Color(0.12f, 0.02f, 0.02f, 0.92f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int boxW = 320;
            int boxH = 240;
            int boxX = (screenW - boxW) / 2;
            int boxY = (screenH - boxH) / 2;

            // Menu Frame
            GUI.color = new Color(0.08f, 0.03f, 0.04f, 0.95f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // Blood Red Accent Border
            GUI.color = new Color(0.9f, 0.15f, 0.15f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 3), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(boxX, boxY + boxH - 3, boxW, 3), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(boxX, boxY + 15, boxW, 30), "<size=18><b><color=#FF2222>☠️ YOU HAVE FALLEN</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(boxX, boxY + 45, boxW, 20), "<size=10><i><color=#BBB>Your spirit returns to the embers of the Monolith.</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            int btnW = boxW - 40;
            int btnH = 36;
            int curY = boxY + 75;

            // 1. Respawn at Campfire
            if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>🔄 Respawn at Village Campfire</b>"))
            {
                RespawnPlayer();
            }
            curY += btnH + 10;

            // 2. Load Save
            bool hasSave = SaveSystem.HasSaveFile;
            GUI.enabled = hasSave;
            if (GUI.Button(new Rect(boxX + 20, curY, btnW, btnH), "<b>📂 Load Last Save</b>"))
            {
                if (SaveSystem.Instance != null) SaveSystem.Instance.LoadGame();
                RespawnPlayer();
            }
            GUI.enabled = true;
            curY += btnH + 10;

            // 3. Return to Main Menu
            if (GUI.Button(new Rect(boxX + 20, curY, btnW, 28), "🏠 Main Menu"))
            {
                RespawnPlayer();
                GameStateManager.Instance.SetState(GameState.MainMenu);
            }
        }

        private void RespawnPlayer()
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.transform.position = new Vector3(0, 0.5f, 2.0f); // Village Plaza
                if (cc != null) cc.enabled = true;
            }

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.Heal(PlayerStats.Instance.MaxHealth);
                PlayerStats.Instance.RestoreMana(PlayerStats.Instance.MaxMana);
            }

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }
        }
    }
}
