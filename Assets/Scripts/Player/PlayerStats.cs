using System;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Core runtime stats container for the Player.
    /// Manages Level, Vitality, Intelligence, Health, Mana, Attack, Defense, and Speed.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private StatsConfig _config;

        [Header("Current Level")]
        [SerializeField] private int _currentLevel = 1;

        // Core Primary Attributes
        public int CurrentLevel => _currentLevel;
        public float Vitality { get; private set; }
        public float Intelligence { get; private set; }

        // Derived Vital Pools
        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public float MaxMana { get; private set; }
        public float CurrentMana { get; private set; }

        // Combat Stats
        public float Attack { get; private set; }
        public float Defense { get; private set; }
        public float Speed { get; private set; }
        public bool IsDead => CurrentHealth <= 0.0f;

        // Stat Modifier Hooks (from Professions / Equipment / Buffs)
        public float BonusVitality { get; set; }
        public float BonusIntelligence { get; set; }
        public float BonusMaxHealth { get; set; }
        public float BonusMaxMana { get; set; }
        public float BonusAttack { get; set; }
        public float BonusDefense { get; set; }
        public float BonusSpeed { get; set; }

        // Events
        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action<float, float> OnManaChanged;   // (current, max)
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

        public void RecalculateStats(bool refillPools = false)
        {
            int levelOffset = Mathf.Max(0, _currentLevel - 1);

            float oldMaxHealth = MaxHealth;
            float oldMaxMana = MaxMana;

            // 1. Calculate Primary Core Attributes
            Vitality = _config.baseVitality + (levelOffset * _config.vitalityGrowthPerLevel) + BonusVitality;
            Intelligence = _config.baseIntelligence + (levelOffset * _config.intelligenceGrowthPerLevel) + BonusIntelligence;

            // 2. Compute Max Health & Max Mana from Attributes
            MaxHealth = _config.baseHealth + (Vitality * _config.healthPerVitality) + BonusMaxHealth;
            MaxMana = _config.baseMana + (Intelligence * _config.manaPerIntelligence) + BonusMaxMana;

            // 3. Compute Combat Stats
            Attack = _config.baseAttack + (levelOffset * _config.attackGrowthPerLevel) + BonusAttack;
            Defense = _config.baseDefense + (levelOffset * _config.defenseGrowthPerLevel) + BonusDefense;
            Speed = _config.baseSpeed + BonusSpeed;

            // 4. Update Current Pools
            if (refillPools || oldMaxHealth <= 0f)
            {
                CurrentHealth = MaxHealth;
                CurrentMana = MaxMana;
            }
            else
            {
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
                CurrentMana = Mathf.Clamp(CurrentMana, 0f, MaxMana);
            }

            OnStatsRecalculated?.Invoke();
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            OnManaChanged?.Invoke(CurrentMana, MaxMana);
        }

        public void TakeDamage(float rawDamage)
        {
            if (IsDead) return;

            // RPG Defense Mitigation formula: EffectiveDamage = RawDamage * (100 / (100 + Defense))
            float damageMultiplier = 100f / (100f + Mathf.Max(0f, Defense));
            float finalDamage = Mathf.Max(1.0f, rawDamage * damageMultiplier);

            CurrentHealth = Mathf.Clamp(CurrentHealth - finalDamage, 0f, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            Debug.Log($"<color=#FF5555>[PlayerStats]</color> Took {finalDamage:F1} dmg. Health: {CurrentHealth:F1}/{MaxHealth:F1}");

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

        public bool UseMana(float cost)
        {
            if (cost <= 0f) return true;

            if (CurrentMana >= cost)
            {
                CurrentMana -= cost;
                OnManaChanged?.Invoke(CurrentMana, MaxMana);
                Debug.Log($"<color=#00D4FF>[PlayerStats]</color> Used {cost:F0} Mana. Remaining: {CurrentMana:F0}/{MaxMana:F0}");
                return true;
            }

            Debug.LogWarning($"<color=#FFAA00>[PlayerStats] Not enough Mana! Required: {cost:F0}, Available: {CurrentMana:F0}</color>");
            return false;
        }

        public void RestoreMana(float amount)
        {
            if (amount <= 0f) return;

            float oldMana = CurrentMana;
            CurrentMana = Mathf.Clamp(CurrentMana + amount, 0f, MaxMana);
            float actual = CurrentMana - oldMana;

            if (actual > 0f)
            {
                OnManaChanged?.Invoke(CurrentMana, MaxMana);
                Debug.Log($"<color=#00D4FF>[PlayerStats]</color> Restored +{actual:F0} Mana. Mana: {CurrentMana:F0}/{MaxMana:F0}");
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
            // Reward: Refill health and mana on level up
            Heal(MaxHealth);
            RestoreMana(MaxMana);
        }

        private void Die()
        {
            Debug.Log("<color=#FF0000>[PlayerStats] Player has Died!</color>");
            OnDeath?.Invoke();
        }
    }
}
