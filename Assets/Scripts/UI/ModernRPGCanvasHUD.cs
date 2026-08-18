using Awakening.Combat;
using Awakening.Core;
using Awakening.Equipment;
using Awakening.Player;
using Awakening.Professions;
using Awakening.Quests;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Modern Dark Fantasy Canvas HUD.
    /// Provides cinematic player health bars, mana pools, XP progression, gold stash,
    /// skill hotbars, and active quest trackers without relying on debug boxes.
    /// </summary>
    public class ModernRPGCanvasHUD : MonoBehaviour
    {
        public static ModernRPGCanvasHUD Instance { get; private set; }

        private PlayerStats _playerStats;
        private PlayerWallet _playerWallet;
        private PlayerProgression _progression;
        private ProfessionSystem _professionSystem;
        private PlayerCombat _playerCombat;

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
            AcquireReferences();
        }

        private void AcquireReferences()
        {
            if (_playerStats == null) _playerStats = PlayerStats.Instance ?? FindFirstObjectByType<PlayerStats>();
            if (_playerWallet == null) _playerWallet = PlayerWallet.Instance ?? FindFirstObjectByType<PlayerWallet>();
            if (_progression == null) _progression = PlayerProgression.Instance ?? FindFirstObjectByType<PlayerProgression>();
            if (_professionSystem == null) _professionSystem = ProfessionSystem.Instance ?? FindFirstObjectByType<ProfessionSystem>();
            if (_playerCombat == null) _playerCombat = PlayerCombat.Instance ?? FindFirstObjectByType<PlayerCombat>();
        }

        private void OnGUI()
        {
            if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                return;
            }

            AcquireReferences();
            if (_playerStats == null) return;

            DrawPlayerTopLeftHUD();
            DrawTopRightStatusHUD();
            DrawBottomCenterSkillBar();
            DrawCenterCrosshair();
        }

        private void DrawPlayerTopLeftHUD()
        {
            int x = 25;
            int y = 25;
            int width = 280;
            int height = 85;

            // Frame Background (Dark Slate)
            GUI.color = new Color(0.05f, 0.07f, 0.1f, 0.85f);
            GUI.DrawTexture(new Rect(x, y, width, height), Texture2D.whiteTexture);

            // Gold Header Border
            GUI.color = new Color(1f, 0.84f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(x, y, width, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Level & Profession Title
            int level = _playerStats.CurrentLevel;
            string profName = _professionSystem != null && _professionSystem.CurrentProfession != null ?
                $"[{_professionSystem.CurrentProfession.rank}] {_professionSystem.CurrentProfession.professionName}" : "Unawakened Hero";

            GUI.Label(new Rect(x + 12, y + 6, width - 24, 20), $"<size=12><b><color=#00D4FF>{profName}</color></b>  <color=#FFD700>Lv.{level}</color></size>");

            // 1. Health Bar (Deep Crimson)
            int barX = x + 12;
            int barY = y + 28;
            int barW = width - 24;
            int barH = 16;

            float hpPct = Mathf.Clamp01(_playerStats.CurrentHealth / Mathf.Max(1f, _playerStats.MaxHealth));
            GUI.color = new Color(0.15f, 0.05f, 0.05f, 0.9f);
            GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture);
            GUI.color = new Color(0.85f, 0.15f, 0.15f, 0.95f);
            GUI.DrawTexture(new Rect(barX, barY, barW * hpPct, barH), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(barX, barY - 1, barW, barH), $"<size=10><b>HP  {_playerStats.CurrentHealth:F0} / {_playerStats.MaxHealth:F0}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // 2. Mana Bar (Arcane Blue)
            int manaY = barY + 20;
            float mpPct = Mathf.Clamp01(_playerStats.CurrentMana / Mathf.Max(1f, _playerStats.MaxMana));
            GUI.color = new Color(0.05f, 0.1f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(barX, manaY, barW, barH), Texture2D.whiteTexture);
            GUI.color = new Color(0.15f, 0.55f, 0.95f, 0.95f);
            GUI.DrawTexture(new Rect(barX, manaY, barW * mpPct, barH), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(barX, manaY - 1, barW, barH), $"<size=10><b>MP  {_playerStats.CurrentMana:F0} / {_playerStats.MaxMana:F0}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // 3. XP Bar (Thin Gold Line at bottom)
            if (_progression != null)
            {
                int xpY = y + height - 4;
                float xpPct = Mathf.Clamp01(_progression.CurrentXP / Mathf.Max(1f, _progression.RequiredXP));
                GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
                GUI.DrawTexture(new Rect(x, xpY, width, 4), Texture2D.whiteTexture);
                GUI.color = new Color(1f, 0.85f, 0.2f, 0.9f);
                GUI.DrawTexture(new Rect(x, xpY, width * xpPct, 4), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
        }

        private void DrawTopRightStatusHUD()
        {
            int screenW = Screen.width;
            int x = screenW - 220;
            int y = 25;
            int w = 195;
            int h = 36;

            // Gold Stash Box
            GUI.color = new Color(0.05f, 0.07f, 0.1f, 0.85f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.84f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(x, y, w, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int gold = _playerWallet != null ? _playerWallet.CurrentGold : 0;
            GUI.Label(new Rect(x + 10, y + 8, w - 20, 20), $"<size=12>💰 <b><color=#FFD700>{gold:N0} Gold</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight });
        }

        private void DrawBottomCenterSkillBar()
        {
            int screenW = Screen.width;
            int screenH = Screen.height;

            int totalW = 340;
            int startX = (screenW - totalW) / 2;
            int startY = screenH - 75;

            // Background tray
            GUI.color = new Color(0.05f, 0.07f, 0.1f, 0.85f);
            GUI.DrawTexture(new Rect(startX, startY, totalW, 60), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.84f, 0.2f, 0.6f);
            GUI.DrawTexture(new Rect(startX, startY, totalW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            int slotW = 52;
            int slotH = 48;
            int gap = 12;
            int curX = startX + 14;
            int curY = startY + 6;

            // Slot 1: Light Attack
            DrawSkillSlot(curX, curY, slotW, slotH, "LMB", "⚔️", "Combo", 0f);
            curX += slotW + gap;

            // Slot 2: Heavy Cleave
            DrawSkillSlot(curX, curY, slotW, slotH, "RMB", "🪓", "Cleave", 0f);
            curX += slotW + gap;

            // Slot 3: Dodge Roll
            DrawSkillSlot(curX, curY, slotW, slotH, "Space", "💨", "Dodge", 0f);
            curX += slotW + gap;

            // Slot 4: Awakened Skill
            float skillCooldown = _playerCombat != null ? _playerCombat.SkillCooldownRemaining : 0f;
            DrawSkillSlot(curX, curY, slotW, slotH, "E", "⚡", "Skill", skillCooldown);
            curX += slotW + gap;

            // Slot 5: Bag
            DrawSkillSlot(curX, curY, slotW, slotH, "I", "🎒", "Bag", 0f);
        }

        private void DrawSkillSlot(int x, int y, int w, int h, string hotkey, string icon, string name, float cooldown)
        {
            // Slot frame
            GUI.color = new Color(0.12f, 0.16f, 0.22f, 0.95f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Icon
            GUI.Label(new Rect(x, y + 2, w, 22), $"<size=15>{icon}</size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // Hotkey Badge
            GUI.Label(new Rect(x, y + 24, w, 14), $"<size=9><b><color=#FFD700>{hotkey}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            // Cooldown Mask
            if (cooldown > 0f)
            {
                GUI.color = new Color(0f, 0f, 0f, 0.75f);
                GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect(x, y + 12, w, 20), $"<size=12><b><color=#FF3333>{cooldown:F1}s</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }

        private void DrawCenterCrosshair()
        {
            int cx = Screen.width / 2;
            int cy = Screen.height / 2;

            GUI.color = new Color(1f, 1f, 1f, 0.6f);
            GUI.DrawTexture(new Rect(cx - 5, cy - 1, 10, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx - 1, cy - 5, 2, 10), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
