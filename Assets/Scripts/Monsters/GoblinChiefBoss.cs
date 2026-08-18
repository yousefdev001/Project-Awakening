using System;
using System.Collections;
using System.Collections.Generic;
using Awakening.Combat;
using Awakening.Core;
using Awakening.Items;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// Goblin Chief World Boss Controller (Phase 24).
    /// Features 2 combat phases, AoE Ground Slam, 50% HP Enrage Mode, and epic victory loot rewards.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Collider))]
    public class GoblinChiefBoss : MonoBehaviour
    {
        public static GoblinChiefBoss Instance { get; private set; }

        [Header("Boss Identity")]
        [SerializeField] private string _bossName = "Gorgar, the Goblin Chief";
        [SerializeField] private int _bossLevel = 10;
        [SerializeField] private float _maxHealth = 600.0f;
        [SerializeField] private float _baseAttack = 32.0f;
        [SerializeField] private float _chaseSpeed = 4.5f;

        [Header("Slam Attack Settings")]
        [SerializeField] private float _slamCooldown = 7.0f;
        [SerializeField] private float _slamRadius = 4.5f;
        [SerializeField] private float _slamDamage = 45.0f;

        public string BossName => _bossName;
        public int BossLevel => _bossLevel;
        public float CurrentHealth => _healthSystem != null ? _healthSystem.CurrentHealth : _maxHealth;
        public float MaxHealth => _maxHealth;
        public bool IsEnraged { get; private set; } = false;
        public bool IsDefeated { get; private set; } = false;

        public event Action<bool> OnEnragedChanged;
        public event Action OnBossDefeated;

        private HealthSystem _healthSystem;
        private Renderer _renderer;
        private Transform _playerTransform;
        private IDamageable _playerDamageable;

        private float _lastAttackTime = -99f;
        private float _lastSlamTime = -99f;
        private bool _isPerformingSlam = false;
        private Color _normalColor = new Color(0.75f, 0.2f, 0.15f); // Deep Rust Red
        private Color _enrageColor = new Color(1.0f, 0.1f, 0.1f); // Blazing Blood Crimson

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _healthSystem = GetComponent<HealthSystem>();
            _renderer = GetComponentInChildren<Renderer>();
        }

        private void Start()
        {
            AcquirePlayer();

            if (_healthSystem != null)
            {
                _healthSystem.Heal(_maxHealth); // Ensure starting at 600 HP
                _healthSystem.OnDamaged += HandleDamaged;
                _healthSystem.OnDeath += HandleDeath;
            }

            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.color = _normalColor;
            }

            transform.localScale = new Vector3(2.2f, 2.6f, 2.2f);
            Debug.Log($"<color=#FF0044>👑 [BOSS SPAWNED]</color> <b>{_bossName}</b> has emerged in the Arena!");
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDamaged -= HandleDamaged;
                _healthSystem.OnDeath -= HandleDeath;
            }
        }

        private void AcquirePlayer()
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                _playerTransform = player.transform;
                _playerDamageable = player.GetComponent<IDamageable>() ?? player.GetComponentInParent<IDamageable>();
            }
        }

        private void Update()
        {
            if (IsDefeated) return;
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay) return;

            if (_playerTransform == null)
            {
                AcquirePlayer();
                return;
            }

            if (_isPerformingSlam) return;

            float distToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Rotate towards player
            Vector3 lookDir = (_playerTransform.position - transform.position);
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * (IsEnraged ? 8f : 5f));
            }

            // Check Ground Slam AoE Attack
            if (Time.time >= _lastSlamTime + (IsEnraged ? 4.5f : _slamCooldown) && distToPlayer <= _slamRadius + 1.0f)
            {
                StartCoroutine(PerformGroundSlamRoutine());
                return;
            }

            // Regular Melee Strike
            float attackRange = 3.0f;
            if (distToPlayer <= attackRange)
            {
                float cd = IsEnraged ? 1.1f : 1.8f;
                if (Time.time >= _lastAttackTime + cd)
                {
                    ExecuteMeleeAttack();
                }
            }
            else
            {
                // Chase player
                float speed = IsEnraged ? _chaseSpeed * 1.4f : _chaseSpeed;
                transform.position += lookDir.normalized * (speed * Time.deltaTime);
            }
        }

        private void ExecuteMeleeAttack()
        {
            _lastAttackTime = Time.time;
            float dmg = IsEnraged ? _baseAttack * 1.5f : _baseAttack;

            if (_playerDamageable != null)
            {
                DamageData data = new DamageData(
                    amount: dmg,
                    damageType: DamageType.Physical,
                    attacker: gameObject,
                    knockbackForce: IsEnraged ? 6.0f : 3.5f
                );

                _playerDamageable.TakeDamage(data);
                Debug.Log($"<color=#FF2200>[Boss Strike]</color> <b>{_bossName}</b> executed Heavy Cleave for <b>{dmg:F1}</b> dmg!");
            }
        }

        private IEnumerator PerformGroundSlamRoutine()
        {
            _isPerformingSlam = true;
            _lastSlamTime = Time.time;

            Debug.Log("<color=#FFD700>💥 [Boss AoE]</color> Gorgar leaps into the air for a <b>GROUND SLAM!</b>");

            // Wind-up: flash yellow and rise slightly
            if (_renderer != null) _renderer.material.color = Color.yellow;
            Vector3 startPos = transform.position;
            transform.position += Vector3.up * 1.2f;

            yield return new WaitForSeconds(0.6f);

            // Slam Down
            transform.position = startPos;
            if (_renderer != null) _renderer.material.color = IsEnraged ? _enrageColor : _normalColor;

            // Damage player if in AoE
            if (_playerTransform != null && _playerDamageable != null)
            {
                float dist = Vector3.Distance(transform.position, _playerTransform.position);
                if (dist <= _slamRadius)
                {
                    DamageData slamData = new DamageData(
                        amount: IsEnraged ? _slamDamage * 1.4f : _slamDamage,
                        damageType: DamageType.Physical,
                        attacker: gameObject,
                        hitPoint: transform.position,
                        knockbackForce: 10.0f
                    );
                    _playerDamageable.TakeDamage(slamData);
                    Debug.Log($"<color=#FF0000>💥 [GROUND SLAM HIT!]</color> Player crushed for <b>{slamData.Amount:F1}</b> damage!");
                }
            }

            yield return new WaitForSeconds(0.4f);
            _isPerformingSlam = false;
        }

        private void HandleDamaged(DamageData data)
        {
            if (IsDefeated) return;

            // Check Enrage Threshold (50% HP)
            if (!IsEnraged && CurrentHealth <= (_maxHealth * 0.5f))
            {
                TriggerEnrage();
            }

            // Brief hurt flash
            StartCoroutine(BossHurtFlashRoutine());
        }

        private void TriggerEnrage()
        {
            IsEnraged = true;

            if (_renderer != null)
            {
                _renderer.material.color = _enrageColor;
            }

            Debug.Log("<color=#FF0000>🔥🔥🔥 [BOSS ENRAGED!] 🔥🔥🔥</color> <b>Gorgar enters Blood Frenzy!</b> Speed +40%, Attack +50%!");
            OnEnragedChanged?.Invoke(true);
        }

        private IEnumerator BossHurtFlashRoutine()
        {
            if (_renderer != null && !_isPerformingSlam)
            {
                _renderer.material.color = Color.white;
                yield return new WaitForSeconds(0.08f);
                _renderer.material.color = IsEnraged ? _enrageColor : _normalColor;
            }
        }

        private void HandleDeath()
        {
            if (IsDefeated) return;
            IsDefeated = true;

            Debug.Log($"<color=#FFD700>👑👑👑 [BOSS DEFEATED!] 👑👑👑</color> <b>{_bossName}</b> has been vanquished!");

            // 1. Award 1000 XP (Level Up!)
            if (PlayerProgression.Instance != null)
            {
                PlayerProgression.Instance.AddXP(1000f);
            }

            // 2. Award 500 Gold
            if (PlayerWallet.Instance != null)
            {
                PlayerWallet.Instance.AddGold(500);
            }

            // 3. Scatter Legendary Drops
            SpawnVictoryLootBurst();

            OnBossDefeated?.Invoke();
            StartCoroutine(BossDeathSequenceRoutine());
        }

        private void SpawnVictoryLootBurst()
        {
            Vector3 origin = transform.position + Vector3.up * 1.0f;

            // Drop 3 Rich Gold Caches
            for (int i = 0; i < 4; i++)
            {
                SpawnDrop(ItemData.CreateGoldPreset(50), 1, origin + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)));
            }

            // Drop Legendary Gear
            SpawnDrop(ItemData.CreateKnightArmorPreset(), 1, origin + new Vector3(1.5f, 0, 0));
            SpawnDrop(ItemData.CreateIronLongswordPreset(), 1, origin + new Vector3(-1.5f, 0, 0));
            SpawnDrop(ItemData.CreateHealthPotionPreset(), 3, origin + new Vector3(0, 0, 1.5f));
            SpawnDrop(ItemData.CreateManaPotionPreset(), 3, origin + new Vector3(0, 0, -1.5f));
        }

        private void SpawnDrop(ItemData item, int qty, Vector3 pos)
        {
            GameObject dropObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dropObj.name = $"BossDrop_{item.itemName.Replace(" ", "_")}";
            dropObj.transform.position = pos;
            dropObj.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

            WorldItemPickup pickup = dropObj.AddComponent<WorldItemPickup>();
            pickup.Setup(item, qty);
        }

        private IEnumerator BossDeathSequenceRoutine()
        {
            float duration = 1.5f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
