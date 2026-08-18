using Awakening.Player;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug visualizer and testing tool for XP and Level Progression.
    /// </summary>
    public class ProgressionDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerProgression prog = PlayerProgression.Instance;
            if (prog == null) return;

            int boxWidth = 260;
            int boxHeight = 160;
            int boxX = 10;
            int boxY = Screen.height - 380;

            GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "⭐ XP & Progression (Phase 8)");

            // Level & XP Bar
            GUI.Label(new Rect(boxX + 10, boxY + 25, boxWidth - 20, 22), $"Level: <color=yellow><b>Lv. {prog.CurrentLevel} / 10</b></color> {(prog.IsMaxLevel ? "<color=#00FFAA>(MAX)</color>" : "")}");

            float xpPercent = prog.RequiredXP > 0 ? Mathf.Clamp01(prog.CurrentXP / prog.RequiredXP) : 1f;
            GUI.Label(new Rect(boxX + 10, boxY + 50, boxWidth - 20, 22), $"XP: <color=#00D4FF><b>{prog.CurrentXP:F0} / {prog.RequiredXP:F0}</b></color> ({(xpPercent * 100):F0}%)");

            // Buttons
            int btnY = boxY + 80;
            if (GUI.Button(new Rect(boxX + 10, btnY, 70, 24), "+50 XP"))
            {
                prog.AddXP(50f);
            }

            if (GUI.Button(new Rect(boxX + 85, btnY, 75, 24), "+250 XP"))
            {
                prog.AddXP(250f);
            }

            if (GUI.Button(new Rect(boxX + 165, btnY, 85, 24), "+1000 XP"))
            {
                prog.AddXP(1000f);
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 32, 115, 24), "Instant Level Up"))
            {
                prog.AddXP(prog.RequiredXP - prog.CurrentXP);
            }

            if (GUI.Button(new Rect(boxX + 135, btnY + 32, 115, 24), "Reset to Lv. 1"))
            {
                prog.ResetProgression();
            }
        }
    }
}
