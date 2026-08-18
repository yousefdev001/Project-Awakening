using System;
using Awakening.Core;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Combat
{
    /// <summary>
    /// Universal health and damage recipient component.
    /// Works for Player (bridging with PlayerStats), Monsters, Bosses, and Destructible objects.
    /// </summary>
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        [Header("Standalone Health Settings (Used if no PlayerStats is attached)")]
        [SerializeField] private float _standaloneMaxHealth = 100f;
        [SerializeField] private float _defense = 0f;

        [Header("Invulnerability Frames (I-Frames)")]
        [SerializeField] private float _iFramesDuration = 0.25f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => _playerStats != null ? _playerStats.MaxHealth : _standaloneMaxHealth;
        public bool IsDead => CurrentHealth <= 0.0f;
        public bool IsInvulnerable => Time.time < _lastDamageTime + _iFramesDuration;

        public event Action<DamageData> OnDamaged;
        public event Action<float> OnHealed;
        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;

        private PlayerStats _playerStats;
        private PlayerAnimation _playerAnimation;
        private float _lastDamageTime = -99f;

        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
            _playerAnimation = GetComponent<PlayerAnimation>();

            CurrentHealth = MaxHealth;
        }

        private void Start()
        {
            if (_playerStats != null)
            {
                // Sync with PlayerStats
                CurrentHealth = _playerStats.CurrentHealth;
                _playerStats.OnHealthChanged += HandlePlayerStatsHealthChanged;
            }
        }

        private void OnDestroy()
        {
            if (_playerStats != null)
            {
                _playerStats.OnHealthChanged -= HandlePlayerStatsHealthChanged;
            }
        }

        private void HandlePlayerStatsHealthChanged(float current, float max)
        {
            CurrentHealth = current;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(DamageData damageData)
        {
            if (IsDead || IsInvulnerable) return;

            _lastDamageTime = Time.time;

            if (_playerStats != null)
            {
                // Delegate to PlayerStats for stat-based damage mitigation and level calculation
                _playerStats.TakeDamage(damageData.Amount);
                CurrentHealth = _playerStats.CurrentHealth;
            }
            else
            {
                // Standalone entity mitigation formula: Effective = Raw * (100 / (100 + Defense))
                float defenseRating = Mathf.Max(0f, _defense);
                float multiplier = damageData.DamageType == DamageType.TrueDamage ? 1f : (100f / (100f + defenseRating));
                float finalDamage = Mathf.Max(1.0f, damageData.Amount * multiplier);

                CurrentHealth = Mathf.Clamp(CurrentHealth - finalDamage, 0f, MaxHealth);
                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

                Debug.Log($"<color=#FF6666>[HealthSystem]</color> {gameObject.name} took {finalDamage:F1} dmg. Health: {CurrentHealth:F1}/{MaxHealth:F1}");

                if (CurrentHealth <= 0.0f)
                {
                    HandleDeath();
                }
            }

            // Spawn Blood Splatter VFX
            VFX.VFXManager.Instance?.SpawnVFX(VFX.VFXType.BloodSplatter, transform.position + Vector3.up * 0.8f);

            // Trigger hit animation if available
            if (_playerAnimation != null && !IsDead)
            {
                _playerAnimation.TriggerHitAnimation();
            }

            OnDamaged?.Invoke(damageData);

            if (_playerStats != null && _playerStats.IsDead)
            {
                HandleDeath();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            if (_playerStats != null)
            {
                _playerStats.Heal(amount);
                CurrentHealth = _playerStats.CurrentHealth;
            }
            else
            {
                float oldHealth = CurrentHealth;
                CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
                float actual = CurrentHealth - oldHealth;

                if (actual > 0f)
                {
                    OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
                    OnHealed?.Invoke(actual);
                }
            }
        }

        private void HandleDeath()
        {
            Debug.Log($"<color=#FF0000>[HealthSystem]</color> <b>{gameObject.name}</b> has perished!");

            if (_playerAnimation != null)
            {
                _playerAnimation.TriggerDieAnimation();
            }

            OnDeath?.Invoke();

            // If player died, transition to GameOver after a short delay
            if (gameObject.CompareTag("Player") || _playerStats != null)
            {
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.SetState(GameState.GameOver);
                }
            }
        }
    }
}
