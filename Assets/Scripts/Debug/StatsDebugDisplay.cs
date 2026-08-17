using Awakening.Player;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to view and manipulate player stats during Play Mode.
    /// </summary>
    public class StatsDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerStats stats = PlayerStats.Instance;
            if (stats == null) return;

            GUI.Box(new Rect(10, Screen.height - 210, 260, 200), "📊 Player Stats (Phase 6)");

            // Level & HP
            GUI.Label(new Rect(20, Screen.height - 185, 240, 22), $"Level: <color=yellow><b>Lv. {stats.CurrentLevel}</b></color>");

            // Health Bar Representation
            float healthPercent = stats.MaxHealth > 0 ? stats.CurrentHealth / stats.MaxHealth : 0;
            string hpColor = healthPercent > 0.5f ? "#55FF55" : (healthPercent > 0.25f ? "#FFFF55" : "#FF5555");
            GUI.Label(new Rect(20, Screen.height - 165, 240, 22), $"Health: <color={hpColor}><b>{stats.CurrentHealth:F0} / {stats.MaxHealth:F0}</b></color> ({(healthPercent * 100):F0}%)");

            // Stats values
            GUI.Label(new Rect(20, Screen.height - 145, 240, 22), $"Attack: <b>{stats.Attack:F1}</b> | Defense: <b>{stats.Defense:F1}</b>");
            GUI.Label(new Rect(20, Screen.height - 125, 240, 22), $"Speed: <b>{stats.Speed:F1}</b>");

            // Test Buttons
            int btnY = Screen.height - 100;
            if (GUI.Button(new Rect(20, btnY, 70, 24), "-20 HP"))
            {
                stats.TakeDamage(20f);
            }

            if (GUI.Button(new Rect(95, btnY, 70, 24), "+25 Heal"))
            {
                stats.Heal(25f);
            }

            if (GUI.Button(new Rect(170, btnY, 90, 24), "Level Up (+1)"))
            {
                stats.LevelUp();
            }

            if (GUI.Button(new Rect(20, btnY + 30, 115, 22), "Reset to Lv. 1"))
            {
                stats.SetLevel(1);
            }

            if (GUI.Button(new Rect(145, btnY + 30, 115, 22), "Max Level (10)"))
            {
                stats.SetLevel(10);
            }
        }
    }
}
