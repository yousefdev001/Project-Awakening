using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Base configuration and growth rates for Player stats (Vitality, Intelligence, Health, Mana, Attack, Defense).
    /// </summary>
    [CreateAssetMenu(fileName = "NewStatsConfig", menuName = "Awakening/Player/Stats Config")]
    public class StatsConfig : ScriptableObject
    {
        [Header("Primary Core Attributes (Level 1)")]
        [Tooltip("Base Vitality: Governs maximum health points")]
        public float baseVitality = 10.0f;

        [Tooltip("Vitality gained per level up")]
        public float vitalityGrowthPerLevel = 2.0f;

        [Tooltip("Health points granted per 1 point of Vitality")]
        public float healthPerVitality = 10.0f;

        [Tooltip("Base Intelligence: Governs maximum mana and magical potency")]
        public float baseIntelligence = 10.0f;

        [Tooltip("Intelligence gained per level up")]
        public float intelligenceGrowthPerLevel = 2.0f;

        [Tooltip("Mana points granted per 1 point of Intelligence")]
        public float manaPerIntelligence = 10.0f;

        [Header("Base Combat Stats (Level 1)")]
        [Tooltip("Initial base maximum health baseline")]
        public float baseHealth = 50.0f;

        [Tooltip("Initial base maximum mana baseline")]
        public float baseMana = 50.0f;

        [Tooltip("Initial base attack power")]
        public float baseAttack = 15.0f;

        [Tooltip("Initial base defense rating")]
        public float baseDefense = 5.0f;

        [Tooltip("Initial base speed multiplier")]
        public float baseSpeed = 5.0f;

        [Header("Growth Rates (Per Level)")]
        [Tooltip("Attack power added per level up")]
        public float attackGrowthPerLevel = 3.0f;

        [Tooltip("Defense added per level up")]
        public float defenseGrowthPerLevel = 1.5f;
    }
}
