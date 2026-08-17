using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Base configuration and growth rates for Player stats.
    /// ScriptableObject for easy balancing without script edits.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStatsConfig", menuName = "Awakening/Player/Stats Config")]
    public class StatsConfig : ScriptableObject
    {
        [Header("Base Stats (Level 1)")]
        [Tooltip("Initial base maximum health")]
        public float baseHealth = 100.0f;

        [Tooltip("Initial base attack power")]
        public float baseAttack = 15.0f;

        [Tooltip("Initial base defense rating")]
        public float baseDefense = 5.0f;

        [Tooltip("Initial base speed multiplier")]
        public float baseSpeed = 5.0f;

        [Header("Growth Rates (Per Level)")]
        [Tooltip("Max health added per level up")]
        public float healthGrowthPerLevel = 15.0f;

        [Tooltip("Attack power added per level up")]
        public float attackGrowthPerLevel = 3.0f;

        [Tooltip("Defense added per level up")]
        public float defenseGrowthPerLevel = 1.5f;
    }
}
