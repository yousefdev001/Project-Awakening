using System;
using System.Collections;
using Awakening.Core;
using Awakening.Input;
using Awakening.Player;
using Awakening.Professions;
using UnityEngine;

namespace Awakening.Combat
{
    /// <summary>
    /// Player Combat Engine managing attack combos, heavy strikes, dodges, and active profession skills.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(HitboxDetector))]
    public class PlayerCombat : MonoBehaviour
    {
        public static PlayerCombat Instance { get; private set; }

        [Header("Combo & Timings")]
        [SerializeField] private float _attackCooldown = 0.45f;
        [SerializeField] private float _heavyAttackCooldown = 0.85f;
        [SerializeField] private float _dodgeCooldown = 0.8f;
        [SerializeField] private float _comboResetWindow = 1.0f;

        [Header("Skill Settings")]
        [SerializeField] private float _skillCooldown = 3.0f;
        [SerializeField] private float _skillManaCost = 25.0f;

        public int CurrentComboIndex { get; private set; } = 0;
        public bool IsAttacking { get; private set; }
        public bool IsDodging { get; private set; }
        public float LastAttackTime { get; private set; } = -99f;
        public float LastSkillTime { get; private set; } = -99f;
        public float SkillCooldownRemaining => Mathf.Max(0f, (LastSkillTime + _skillCooldown) - Time.time);

        public event Action<int, float> OnAttackExecuted; // (comboIndex, damage)
        public event Action<float> OnHeavyAttackExecuted; // (damage)
        public event Action OnDodgeExecuted;
        public event Action<string, float> OnSkillExecuted; // (skillName, damage)

        private IInputProvider _inputProvider;
        private PlayerStats _playerStats;
        private PlayerAnimation _playerAnimation;
        private PlayerMovement _playerMovement;
        private HitboxDetector _hitboxDetector;
        private ProfessionSystem _professionSystem;
        private HealthSystem _healthSystem;

        private float _lastDodgeTime = -99f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _playerStats = GetComponent<PlayerStats>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _playerMovement = GetComponent<PlayerMovement>();
            _hitboxDetector = GetComponent<HitboxDetector>();
            _professionSystem = GetComponent<ProfessionSystem>();
            _healthSystem = GetComponent<HealthSystem>();
        }

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;

            if (_inputProvider != null)
            {
                _inputProvider.OnAttack += HandleLightAttack;
                _inputProvider.OnHeavyAttack += HandleHeavyAttack;
                _inputProvider.OnDodge += HandleDodge;
                _inputProvider.OnSkill += HandleSkill;
            }
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnAttack -= HandleLightAttack;
                _inputProvider.OnHeavyAttack -= HandleHeavyAttack;
                _inputProvider.OnDodge -= HandleDodge;
                _inputProvider.OnSkill -= HandleSkill;
            }
        }

        private void Update()
        {
            // Reset combo string if player stops attacking past the window
            if (CurrentComboIndex > 0 && Time.time > LastAttackTime + _comboResetWindow)
            {
                CurrentComboIndex = 0;
            }
        }

        private bool CanPerformCombatAction()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                return false;
            }

            if (_healthSystem != null && _healthSystem.IsDead)
            {
                return false;
            }

            return !IsDodging;
        }

        public void HandleLightAttack()
        {
            if (!CanPerformCombatAction()) return;
            if (Time.time < LastAttackTime + _attackCooldown) return;

            LastAttackTime = Time.time;
            CurrentComboIndex = (CurrentComboIndex % 3) + 1; // 1 -> 2 -> 3 -> 1

            // Multipliers: Combo 1: 100%, Combo 2: 120%, Combo 3: 150% finisher
            float multiplier = CurrentComboIndex == 3 ? 1.5f : (CurrentComboIndex == 2 ? 1.2f : 1.0f);
            float baseDamage = _playerStats != null ? _playerStats.Attack : 15.0f;
            float finalDamage = baseDamage * multiplier;

            // Trigger Animation
            if (_playerAnimation != null)
            {
                _playerAnimation.TriggerAttackAnimation();
            }

            // Damage Targets via Hitbox
            DamageData data = new DamageData(
                amount: finalDamage,
                damageType: DamageType.Physical,
                attacker: gameObject,
                isCritical: CurrentComboIndex == 3,
                knockbackForce: CurrentComboIndex == 3 ? 4.0f : 1.5f
            );

            var hits = _hitboxDetector.DetectAndDamageTargets(data);

            Debug.Log($"<color=#FF8800>[Combat]</color> Light Attack ({CurrentComboIndex}/3)! Dealt <b>{finalDamage:F1}</b> dmg to {hits.Count} target(s).");
            OnAttackExecuted?.Invoke(CurrentComboIndex, finalDamage);
        }

        public void HandleHeavyAttack()
        {
            if (!CanPerformCombatAction()) return;
            if (Time.time < LastAttackTime + _heavyAttackCooldown) return;

            LastAttackTime = Time.time;
            CurrentComboIndex = 0; // Heavy attack breaks normal combo string

            float baseDamage = _playerStats != null ? _playerStats.Attack : 15.0f;
            float finalDamage = baseDamage * 2.2f; // Heavy strike deals 220%

            if (_playerAnimation != null)
            {
                _playerAnimation.TriggerHeavyAttackAnimation();
            }

            DamageData data = new DamageData(
                amount: finalDamage,
                damageType: DamageType.Physical,
                attacker: gameObject,
                isCritical: true,
                knockbackForce: 7.0f // Strong Knockback
            );

            var hits = _hitboxDetector.DetectAndDamageTargets(data);

            Debug.Log($"<color=#FF2200>[Combat] ★ HEAVY ATTACK! ★</color> Dealt <b>{finalDamage:F1}</b> dmg to {hits.Count} target(s).");
            OnHeavyAttackExecuted?.Invoke(finalDamage);
        }

        public void HandleDodge()
        {
            if (!CanPerformCombatAction()) return;
            if (Time.time < _lastDodgeTime + _dodgeCooldown) return;

            _lastDodgeTime = Time.time;
            StartCoroutine(DodgeRoutine());
        }

        private IEnumerator DodgeRoutine()
        {
            IsDodging = true;

            if (_playerAnimation != null)
            {
                _playerAnimation.TriggerDodgeAnimation();
            }

            Debug.Log("<color=#00D4FF>[Combat] 💨 DODGE EXECUTED (I-Frames Active)!</color>");
            OnDodgeExecuted?.Invoke();

            yield return new WaitForSeconds(0.4f);

            IsDodging = false;
        }

        public void HandleSkill()
        {
            if (!CanPerformCombatAction()) return;
            if (SkillCooldownRemaining > 0f)
            {
                Debug.LogWarning($"[Combat] Skill on cooldown! Wait {SkillCooldownRemaining:F1}s");
                return;
            }

            // Check Mana
            if (_playerStats != null && !_playerStats.UseMana(_skillManaCost))
            {
                return; // Not enough mana
            }

            LastSkillTime = Time.time;

            ProfessionData prof = _professionSystem != null ? _professionSystem.CurrentProfession : null;
            string skillName = prof != null ? prof.skillName : "Basic Power Strike";
            float baseDamage = _playerStats != null ? _playerStats.Attack : 15.0f;
            float skillDamage = baseDamage * 2.5f;
            DamageType dmgType = (prof != null && prof.weaponAffinity == "Staff") ? DamageType.Magical : DamageType.Physical;

            DamageData data = new DamageData(
                amount: skillDamage,
                damageType: dmgType,
                attacker: gameObject,
                isCritical: true,
                knockbackForce: 8.0f
            );

            var hits = _hitboxDetector.DetectAndDamageTargets(data);

            Debug.Log($"<color=#AA00FF>[Combat] 🔮 SKILL ACTIVATED: [{skillName}]! Dealt {skillDamage:F1} {dmgType} dmg.</color>");
            OnSkillExecuted?.Invoke(skillName, skillDamage);
        }
    }
}
