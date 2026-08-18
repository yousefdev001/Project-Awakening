using UnityEngine;

namespace Awakening.Professions
{
    /// <summary>
    /// Configuration for Awakening Profession RNG weights and pool assignments.
    /// ScriptableObject enables live tuning of drop chances.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRandomizerConfig", menuName = "Awakening/Professions/Randomizer Config")]
    public class RandomizerConfig : ScriptableObject
    {
        [Header("Probability Distribution (Must Sum to 100%)")]
        [Tooltip("Percentage chance for Rank C (Swordsman) in MVP")]
        [Range(0f, 100f)] public float rankCWeight = 60.0f;

        [Tooltip("Percentage chance for Rank B (Hunter) in MVP")]
        [Range(0f, 100f)] public float rankBWeight = 30.0f;

        [Tooltip("Percentage chance for Rank A (Battle Mage) in MVP")]
        [Range(0f, 100f)] public float rankAWeight = 10.0f;

        [Header("Optional Custom Asset References (Uses built-in presets if left empty)")]
        public ProfessionData customSwordsman;
        public ProfessionData customHunter;
        public ProfessionData customBattleMage;
    }
}
