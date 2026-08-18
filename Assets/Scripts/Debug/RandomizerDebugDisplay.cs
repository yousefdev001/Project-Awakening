using Awakening.Professions;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to test rolling random professions and batch simulations in Play Mode.
    /// </summary>
    public class RandomizerDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private string _lastRollInfo = "None";
        private float _lastRollTime = 0f;

        private void Start()
        {
            if (ProfessionRandomizer.Instance != null)
            {
                ProfessionRandomizer.Instance.OnProfessionRolled += (data, roll) =>
                {
                    _lastRollInfo = $"[{data.rank}] {data.professionName} (Roll: {roll:F1}%)";
                    _lastRollTime = Time.time;
                };
            }
        }

        private void OnGUI()
        {
            if (!_showUI) return;

            ProfessionRandomizer rando = ProfessionRandomizer.Instance;
            if (rando == null) return;

            int boxW = 260;
            int boxH = 150;
            int boxX = Screen.width - 270;
            int boxY = Screen.height - 390;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🎲 Profession Randomizer (Phase 10)");

            GUI.Label(new Rect(boxX + 10, boxY + 25, boxW - 20, 22), $"Last Roll: <b><color=yellow>{_lastRollInfo}</color></b>");
            GUI.Label(new Rect(boxX + 10, boxY + 45, boxW - 20, 22), "<size=10>Odds: Rank C: 60% | Rank B: 30% | Rank A: 10%</size>");

            int btnY = boxY + 75;
            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, 30), "🎲 Roll Awakening Profession"))
            {
                rando.RollAndAssignProfession();
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 35, boxW - 20, 22), "📊 Simulate 100 Rolls (Console Log)"))
            {
                rando.SimulateBatchRolls(100);
            }
        }
    }
}
