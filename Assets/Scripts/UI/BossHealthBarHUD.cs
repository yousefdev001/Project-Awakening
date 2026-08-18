using Awakening.Monsters;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Cinematic Boss Health Bar HUD displayed at the top of the screen during Boss Encounters.
    /// Includes animated health bar, Enrage mode indicator, and Victory fanfare banner.
    /// </summary>
    public class BossHealthBarHUD : MonoBehaviour
    {
        [SerializeField] private bool _showHUD = true;

        private float _victoryBannerTimer = 0f;
        private bool _showVictoryBanner = false;

        private void Start()
        {
            if (GoblinChiefBoss.Instance != null)
            {
                GoblinChiefBoss.Instance.OnBossDefeated += HandleBossDefeated;
            }
        }

        private void OnEnable()
        {
            if (GoblinChiefBoss.Instance != null)
            {
                GoblinChiefBoss.Instance.OnBossDefeated += HandleBossDefeated;
            }
        }

        private void HandleBossDefeated()
        {
            _showVictoryBanner = true;
            _victoryBannerTimer = 7.0f;
        }

        private void Update()
        {
            if (_victoryBannerTimer > 0f)
            {
                _victoryBannerTimer -= Time.deltaTime;
                if (_victoryBannerTimer <= 0f)
                {
                    _showVictoryBanner = false;
                }
            }
        }

        private void OnGUI()
        {
            if (!_showHUD) return;

            GoblinChiefBoss boss = GoblinChiefBoss.Instance;

            // 1. Draw Active Boss Health Bar
            if (boss != null && !boss.IsDefeated)
            {
                DrawBossHealthBar(boss);
            }

            // 2. Draw Victory Banner
            if (_showVictoryBanner)
            {
                DrawVictoryBanner();
            }
        }

        private void DrawBossHealthBar(GoblinChiefBoss boss)
        {
            int screenW = Screen.width;
            int barW = Mathf.Min(520, screenW - 40);
            int barH = 50;
            int barX = (screenW - barW) / 2;
            int barY = 18;

            // Background banner
            GUI.color = new Color(0.06f, 0.03f, 0.04f, 0.9f);
            GUI.DrawTexture(new Rect(barX, barY, barW, barH), Texture2D.whiteTexture);

            // Gold/Red Accent Border
            GUI.color = boss.IsEnraged ? new Color(1f, 0.2f, 0.1f) : new Color(1f, 0.84f, 0.2f);
            GUI.DrawTexture(new Rect(barX, barY, barW, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(barX, barY + barH - 2, barW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Boss Title & Level
            string enrageBadge = boss.IsEnraged ? " <color=#FF2200>[🔥 ENRAGED PHASE 2]</color>" : "";
            GUI.Label(new Rect(barX + 15, barY + 5, barW - 30, 20), $"<size=12><b><color=#FFD700>👑 {boss.BossName.ToUpper()}</color></b> [Lv. {boss.BossLevel} BOSS]{enrageBadge}</size>");

            // Health Value Text
            GUI.Label(new Rect(barX + barW - 140, barY + 5, 125, 20), $"<size=11><b>{boss.CurrentHealth:F0} / {boss.MaxHealth:F0} HP</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight });

            // Health Bar Fill
            int fillX = barX + 15;
            int fillY = barY + 28;
            int fillW = barW - 30;
            int fillH = 14;

            // Bar background slot
            GUI.color = new Color(0.2f, 0.1f, 0.1f);
            GUI.DrawTexture(new Rect(fillX, fillY, fillW, fillH), Texture2D.whiteTexture);

            // Bar Foreground Fill
            float hpPercent = Mathf.Clamp01(boss.CurrentHealth / boss.MaxHealth);
            GUI.color = boss.IsEnraged ? new Color(1f, 0.15f, 0.1f) : new Color(0.85f, 0.2f, 0.2f);
            GUI.DrawTexture(new Rect(fillX, fillY, fillW * hpPercent, fillH), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private void DrawVictoryBanner()
        {
            int screenW = Screen.width;
            int bannerW = 560;
            int bannerH = 90;
            int bannerX = (screenW - bannerW) / 2;
            int bannerY = 120;

            // Victory banner backdrop
            GUI.color = new Color(0.08f, 0.05f, 0.02f, 0.95f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, bannerH), Texture2D.whiteTexture);

            // Double Gold borders
            GUI.color = new Color(1f, 0.85f, 0.2f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, 3), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(bannerX, bannerY + bannerH - 3, bannerW, 3), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(bannerX, bannerY + 12, bannerW, 32), "<size=17><b><color=#FFD700>👑 VICTORY! THE GOBLIN CHIEF HAS FALLEN! 👑</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(bannerX, bannerY + 46, bannerW, 26), "<size=12><b><color=#00FFAA>+1000 XP</color></b> | <b><color=#FFD700>+500 Gold</color></b> | <b><color=#FF8800>Legendary Spoils of War Claimed!</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
