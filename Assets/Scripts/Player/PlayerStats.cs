using System;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Core runtime stats container for the Player.
    /// Manages Level, Health, Attack, Defense, Speed, and stat recalculation.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private StatsConfig _config;

        [Header("Current Level")]
        [SerializeField] private int _currentLevel = 1;

        // Computed Stats
        public int CurrentLevel => _currentLevel;
        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public float Attack { get; private set; }
        public float Defense { get; private set; }
        public float Speed { get; private set; }
        public bool IsDead => CurrentHealth <= 0.0f;

        // Stat Modifier Hooks (for Professions / Equipment / Buffs)
        public float BonusMaxHealth { get; set; }
        public float BonusAttack { get; set; }
        public float BonusDefense { get; set; }
        public float BonusSpeed { get; set; }

        // Events
        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action<int> OnLevelChanged;           // (newLevel)
        public event Action OnStatsRecalculated;
        public event Action OnDeath;

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
                _config = ScriptableObject.CreateInstance<StatsConfig>();
            }

            RecalculateStats(true);
        }

        public void RecalculateStats(bool refillHealth = false)
        {
            int levelOffset = Mathf.Max(0, _currentLevel - 1);

            float oldMaxHealth = MaxHealth;

            MaxHealth = _config.baseHealth + (levelOffset * _config.healthGrowthPerLevel) + BonusMaxHealth;
            Attack = _config.baseAttack + (levelOffset * _config.attackGrowthPerLevel) + BonusAttack;
            Defense = _config.baseDefense + (levelOffset * _config.defenseGrowthPerLevel) + BonusDefense;
            Speed = _config.baseSpeed + BonusSpeed;

            if (refillHealth || oldMaxHealth <= 0f)
            {
                CurrentHealth = MaxHealth;
            }
            else
            {
                // Maintain health percentage or clamp to new max
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
            }

            OnStatsRecalculated?.Invoke();
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(float rawDamage)
        {
            if (IsDead) return;

            // RPG Defense Mitigation formula: EffectiveDamage = RawDamage * (100 / (100 + Defense))
            float damageMultiplier = 100f / (100f + Mathf.Max(0f, Defense));
            float finalDamage = Mathf.Max(1.0f, rawDamage * damageMultiplier);

            CurrentHealth = Mathf.Clamp(CurrentHealth - finalDamage, 0f, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            Debug.Log($"<color=#FF5555>[PlayerStats]</color> Took {finalDamage:F1} dmg (Raw: {rawDamage}). Health: {CurrentHealth:F1}/{MaxHealth:F1}");

            if (CurrentHealth <= 0.0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            float oldHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
            float actualHeal = CurrentHealth - oldHealth;

            if (actualHeal > 0f)
            {
                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
                Debug.Log($"<color=#55FF55>[PlayerStats]</color> Healed +{actualHeal:F1} HP. Health: {CurrentHealth:F1}/{MaxHealth:F1}");
            }
        }

        public void SetLevel(int level)
        {
            if (level == _currentLevel) return;

            _currentLevel = Mathf.Max(1, level);
            RecalculateStats(false);
            OnLevelChanged?.Invoke(_currentLevel);

            Debug.Log($"<color=#FFD700>[PlayerStats]</color> Level changed to <b>{_currentLevel}</b>!");
        }

        public void LevelUp()
        {
            SetLevel(_currentLevel + 1);
            // Reward: Refill health on level up
            Heal(MaxHealth);
        }

        private void Die()
        {
            Debug.Log("<color=#FF0000>[PlayerStats] Player has Died!</color>");
            OnDeath?.Invoke();
        }
    }
}
