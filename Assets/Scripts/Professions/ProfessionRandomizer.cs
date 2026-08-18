using System;
using UnityEngine;

namespace Awakening.Professions
{
    /// <summary>
    /// Core RNG engine for the Profession Awakening Ceremony.
    /// Rolls a weighted random profession according to Rank distribution (C:60%, B:30%, A:10%).
    /// </summary>
    public class ProfessionRandomizer : MonoBehaviour
    {
        public static ProfessionRandomizer Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private RandomizerConfig _config;

        public event Action<ProfessionData, float> OnProfessionRolled; // (data, rolledPercentage)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<RandomizerConfig>();
            }
        }

        /// <summary>
        /// Rolls a random profession using configured probability weights and assigns it to the player.
        /// </summary>
        public ProfessionData RollAndAssignProfession()
        {
            float roll = UnityEngine.Random.Range(0.0f, 100.0f);
            ProfessionData selectedProfession;

            float thresholdC = _config.rankCWeight;
            float thresholdB = thresholdC + _config.rankBWeight;

            if (roll < thresholdC)
            {
                // Rank C (60% chance)
                selectedProfession = _config.customSwordsman != null
                    ? _config.customSwordsman
                    : ProfessionSystem.CreateSwordsmanPreset();
            }
            else if (roll < thresholdB)
            {
                // Rank B (30% chance)
                selectedProfession = _config.customHunter != null
                    ? _config.customHunter
                    : ProfessionSystem.CreateHunterPreset();
            }
            else
            {
                // Rank A (10% chance)
                selectedProfession = _config.customBattleMage != null
                    ? _config.customBattleMage
                    : ProfessionSystem.CreateBattleMagePreset();
            }

            Debug.Log($"<color=#FFD700>[ProfessionRandomizer]</color> Rolled: <b>{roll:F1}%</b> ➔ <b>[{selectedProfession.rank}] {selectedProfession.professionName}</b>");

            if (ProfessionSystem.Instance != null)
            {
                ProfessionSystem.Instance.AssignProfession(selectedProfession);
            }

            OnProfessionRolled?.Invoke(selectedProfession, roll);

            return selectedProfession;
        }

        /// <summary>
        /// Simulates multiple rolls for statistical balance verification.
        /// </summary>
        public void SimulateBatchRolls(int totalRolls = 100)
        {
            int countC = 0;
            int countB = 0;
            int countA = 0;

            for (int i = 0; i < totalRolls; i++)
            {
                float roll = UnityEngine.Random.Range(0f, 100f);
                float thresholdC = _config.rankCWeight;
                float thresholdB = thresholdC + _config.rankBWeight;

                if (roll < thresholdC) countC++;
                else if (roll < thresholdB) countB++;
                else countA++;
            }

            Debug.Log($"<color=#00FFAA>📊 [Randomizer Simulation {totalRolls} Rolls]</color>\n" +
                      $"Rank C (Swordsman): {countC} ({countC * 100f / totalRolls:F1}%)\n" +
                      $"Rank B (Hunter): {countB} ({countB * 100f / totalRolls:F1}%)\n" +
                      $"Rank A (Battle Mage): {countA} ({countA * 100f / totalRolls:F1}%)");
        }
    }
}
