using System;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Core Player Progression manager. Handles XP accumulation, level ups, and curves.
    /// Integrates directly with PlayerStats.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerProgression : MonoBehaviour
    {
        public static PlayerProgression Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private ProgressionConfig _config;

        public float CurrentXP { get; private set; }
        public float RequiredXP { get; private set; }
        public float TotalAccumulatedXP { get; private set; }
        public int CurrentLevel => _playerStats != null ? _playerStats.CurrentLevel : 1;
        public bool IsMaxLevel => _config != null && CurrentLevel >= _config.maxLevel;

        public event Action<float, float> OnXPChanged; // (currentXP, requiredXP)
        public event Action<int> OnLevelUp;           // (newLevel)
        public event Action OnMaxLevelReached;

        private PlayerStats _playerStats;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _playerStats = GetComponent<PlayerStats>();

            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<ProgressionConfig>();
            }

            UpdateRequiredXP();
        }

        private void Start()
        {
            OnXPChanged?.Invoke(CurrentXP, RequiredXP);
        }

        public void AddXP(float amount)
        {
            if (amount <= 0f || IsMaxLevel) return;

            CurrentXP += amount;
            TotalAccumulatedXP += amount;

            Debug.Log($"<color=#FFD700>[Progression]</color> Gained +{amount:F0} XP. Total Level XP: {CurrentXP:F0}/{RequiredXP:F0}");

            CheckForLevelUp();

            OnXPChanged?.Invoke(CurrentXP, RequiredXP);
        }

        private void CheckForLevelUp()
        {
            while (CurrentXP >= RequiredXP && !IsMaxLevel)
            {
                CurrentXP -= RequiredXP;

                // Level up in PlayerStats (stat growths & health refill)
                _playerStats.LevelUp();

                UpdateRequiredXP();

                Debug.Log($"<color=#00FFAA>[Progression] ★ LEVEL UP! Reached Level {_playerStats.CurrentLevel} ★</color>");

                OnLevelUp?.Invoke(_playerStats.CurrentLevel);

                if (IsMaxLevel)
                {
                    CurrentXP = 0f;
                    OnMaxLevelReached?.Invoke();
                    Debug.Log("<color=#FFD700>[Progression] MAX LEVEL REACHED! (Level 10)</color>");
                    break;
                }
            }
        }

        private void UpdateRequiredXP()
        {
            RequiredXP = _config.GetRequiredXP(CurrentLevel);
        }

        public void ResetProgression()
        {
            CurrentXP = 0f;
            TotalAccumulatedXP = 0f;
            _playerStats.SetLevel(1);
            UpdateRequiredXP();
            OnXPChanged?.Invoke(CurrentXP, RequiredXP);
        }
    }
}
