using Awakening.Player;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to view and manipulate player stats (Health, Mana, Vitality, Intelligence, Gold) during Play Mode.
    /// </summary>
    public class StatsDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerStats stats = PlayerStats.Instance;
            if (stats == null) return;

            PlayerWallet wallet = PlayerWallet.Instance;
            int gold = wallet != null ? wallet.Gold : 0;

            int boxW = 270;
            int boxH = 265;
            int boxX = 10;
            int boxY = Screen.height - 275;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "📊 Player Stats & Attributes (Phase 6)");

            // Level & Gold & Primary Attributes
            GUI.Label(new Rect(boxX + 10, boxY + 22, boxW - 20, 20), $"Level: <color=yellow><b>Lv. {stats.CurrentLevel}</b></color> | 💰 <b><color=#FFD700>{gold} Gold</color></b>");
            GUI.Label(new Rect(boxX + 10, boxY + 42, boxW - 20, 20), $"Vitality: <b>{stats.Vitality:F0}</b> | Intelligence: <b>{stats.Intelligence:F0}</b>");

            // Health Bar Representation
            float healthPercent = stats.MaxHealth > 0 ? stats.CurrentHealth / stats.MaxHealth : 0;
            string hpColor = healthPercent > 0.5f ? "#55FF55" : (healthPercent > 0.25f ? "#FFFF55" : "#FF5555");
            GUI.Label(new Rect(boxX + 10, boxY + 64, boxW - 20, 20), $"HP: <color={hpColor}><b>{stats.CurrentHealth:F0} / {stats.MaxHealth:F0}</b></color> ({(healthPercent * 100):F0}%)");

            // Mana Bar Representation
            float manaPercent = stats.MaxMana > 0 ? stats.CurrentMana / stats.MaxMana : 0;
            GUI.Label(new Rect(boxX + 10, boxY + 86, boxW - 20, 20), $"MP: <color=#00D4FF><b>{stats.CurrentMana:F0} / {stats.MaxMana:F0}</b></color> ({(manaPercent * 100):F0}%)");

            // Combat stats values
            GUI.Label(new Rect(boxX + 10, boxY + 108, boxW - 20, 20), $"Atk: <b>{stats.Attack:F1}</b> | Def: <b>{stats.Defense:F1}</b> | Spd: <b>{stats.Speed:F1}</b>");

            // Test Buttons Row 1: Health
            int btnY = boxY + 134;
            if (GUI.Button(new Rect(boxX + 10, btnY, 70, 24), "-25 HP"))
            {
                stats.TakeDamage(25f);
            }

            if (GUI.Button(new Rect(boxX + 85, btnY, 75, 24), "+30 Heal"))
            {
                stats.Heal(30f);
            }

            if (GUI.Button(new Rect(boxX + 165, btnY, 95, 24), "Level Up (+1)"))
            {
                stats.LevelUp();
            }

            // Test Buttons Row 2: Mana
            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 70, 24), "-25 MP"))
            {
                stats.UseMana(25f);
            }

            if (GUI.Button(new Rect(boxX + 85, btnY2, 75, 24), "+30 MP"))
            {
                stats.RestoreMana(30f);
            }

            if (GUI.Button(new Rect(boxX + 165, btnY2, 95, 24), "Full Restore"))
            {
                stats.Heal(stats.MaxHealth);
                stats.RestoreMana(stats.MaxMana);
            }

            // Reset Level Button
            int btnY3 = btnY2 + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY3, 120, 22), "Reset to Lv. 1"))
            {
                stats.SetLevel(1);
            }

            if (GUI.Button(new Rect(boxX + 140, btnY3, 120, 22), "Max Level (10)"))
            {
                stats.SetLevel(10);
            }
        }
    }
}
