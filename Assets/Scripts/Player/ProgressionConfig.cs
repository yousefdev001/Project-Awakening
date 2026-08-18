using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Configuration data for player XP requirements and Level Progression curve.
    /// ScriptableObject for easy balancing.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProgressionConfig", menuName = "Awakening/Player/Progression Config")]
    public class ProgressionConfig : ScriptableObject
    {
        [Header("Level Caps")]
        [Tooltip("Maximum achievable level in the MVP")]
        public int maxLevel = 10;

        [Header("XP Formula: RequiredXP = baseXP * (level ^ xpExponent)")]
        [Tooltip("Base XP required to advance from Level 1 to Level 2")]
        public float baseXP = 100f;

        [Tooltip("Exponent controlling the steepness of the XP curve")]
        public float xpExponent = 1.45f;

        /// <summary>
        /// Calculates total XP required to advance from the given level to the next.
        /// </summary>
        public float GetRequiredXP(int currentLevel)
        {
            if (currentLevel >= maxLevel) return float.MaxValue;
            return Mathf.Round(baseXP * Mathf.Pow(currentLevel, xpExponent));
        }
    }
}
