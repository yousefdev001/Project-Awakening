using System.Collections;
using Awakening.Combat;
using Awakening.Core;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// Core AI Controller driving monster state machine, patrol, chase navigation, and attack execution.
    /// </summary>
    [RequireComponent(typeof(MonsterStats))]
    [RequireComponent(typeof(MonsterCombat))]
    [RequireComponent(typeof(HealthSystem))]
    public class MonsterController : MonoBehaviour
    {
        [Header("Runtime State (Read Only)")]
        [SerializeField] private MonsterAIState _currentState = MonsterAIState.Idle;

        public MonsterAIState CurrentState => _currentState;

        private MonsterStats _stats;
        private MonsterCombat _combat;
        private HealthSystem _healthSystem;

        private Transform _playerTarget;
        private IDamageable _playerDamageable;
        private Vector3 _spawnPosition;
        private Vector3 _patrolDestination;
        private float _stateTimer = 0f;
        private bool _isHurt = false;

        private void Awake()
        {
            _stats = GetComponent<MonsterStats>();
            _combat = GetComponent<MonsterCombat>();
            _healthSystem = GetComponent<HealthSystem>();
            _spawnPosition = transform.position;
        }

        private void Start()
        {
            AcquirePlayerReference();

            if (_healthSystem != null)
            {
                _healthSystem.OnDamaged += HandleDamaged;
                _healthSystem.OnDeath += HandleDeath;
            }

            SetState(MonsterAIState.Idle);
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDamaged -= HandleDamaged;
                _healthSystem.OnDeath -= HandleDeath;
            }
        }

        private void AcquirePlayerReference()
        {
            PlayerMovement playerMov = FindFirstObjectByType<PlayerMovement>();
            if (playerMov != null)
            {
                _playerTarget = playerMov.transform;
                _playerDamageable = playerMov.GetComponent<IDamageable>() ?? playerMov.GetComponentInParent<IDamageable>();
            }
        }

        private void Update()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                return;
            }

            if (_currentState == MonsterAIState.Death) return;

            if (_playerTarget == null)
            {
                AcquirePlayerReference();
            }

            _stateTimer += Time.deltaTime;

            switch (_currentState)
            {
                case MonsterAIState.Idle:
                    UpdateIdleState();
                    break;
                case MonsterAIState.Patrol:
                    UpdatePatrolState();
                    break;
                case MonsterAIState.Chase:
                    UpdateChaseState();
                    break;
                case MonsterAIState.Attack:
                    UpdateAttackState();
                    break;
                case MonsterAIState.Hurt:
                    // Handled in Coroutine
                    break;
            }
        }

        public void SetState(MonsterAIState newState)
        {
            if (_currentState == MonsterAIState.Death && newState != MonsterAIState.Death) return;

            _currentState = newState;
            _stateTimer = 0f;

            if (_currentState == MonsterAIState.Patrol)
            {
                // Pick random patrol point within 4m of spawn
                Vector2 randCircle = Random.insideUnitCircle * 4.0f;
                _patrolDestination = _spawnPosition + new Vector3(randCircle.x, 0f, randCircle.y);
            }
        }

        private void UpdateIdleState()
        {
            // Check for player detection
            if (IsPlayerDetected())
            {
                SetState(MonsterAIState.Chase);
                return;
            }

            // Switch to patrol after idle time
            if (_stateTimer > 2.5f)
            {
                SetState(MonsterAIState.Patrol);
            }
        }

        private void UpdatePatrolState()
        {
            if (IsPlayerDetected())
            {
                SetState(MonsterAIState.Chase);
                return;
            }

            // Move towards patrol destination
            MoveTowardsPosition(_patrolDestination, _stats.MoveSpeed);

            if (Vector3.Distance(transform.position, _patrolDestination) < 0.5f || _stateTimer > 4.0f)
            {
                SetState(MonsterAIState.Idle);
            }
        }

        private void UpdateChaseState()
        {
            if (_playerTarget == null)
            {
                SetState(MonsterAIState.Idle);
                return;
            }

            float distToPlayer = Vector3.Distance(transform.position, _playerTarget.position);

            // In Attack range?
            if (distToPlayer <= _stats.AttackRange)
            {
                SetState(MonsterAIState.Attack);
                return;
            }

            // Lost player beyond detection range + 4m buffer
            if (distToPlayer > _stats.DetectionRadius + 4.0f)
            {
                SetState(MonsterAIState.Idle);
                return;
            }

            // Chase player
            MoveTowardsPosition(_playerTarget.position, _stats.ChaseSpeed);
        }

        private void UpdateAttackState()
        {
            if (_playerTarget == null)
            {
                SetState(MonsterAIState.Idle);
                return;
            }

            // Face player during attack
            FaceTarget(_playerTarget.position);

            float distToPlayer = Vector3.Distance(transform.position, _playerTarget.position);

            if (distToPlayer > _stats.AttackRange + 0.5f)
            {
                SetState(MonsterAIState.Chase);
                return;
            }

            // Attempt strike
            if (_playerDamageable != null)
            {
                _combat.TryAttackTarget(_playerDamageable);
            }
        }

        private void MoveTowardsPosition(Vector3 targetPos, float speed)
        {
            Vector3 direction = (targetPos - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                FaceTarget(targetPos);
                transform.position += direction.normalized * (speed * Time.deltaTime);
            }
        }

        private void FaceTarget(Vector3 targetPos)
        {
            Vector3 lookDirection = (targetPos - transform.position);
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10.0f);
            }
        }

        private bool IsPlayerDetected()
        {
            if (_playerTarget == null) return false;

            float distSqr = (transform.position - _playerTarget.position).sqrMagnitude;
            return distSqr <= (_stats.DetectionRadius * _stats.DetectionRadius);
        }

        private void HandleDamaged(DamageData data)
        {
            if (_currentState == MonsterAIState.Death) return;

            // Aggro on attacker immediately
            if (data.Attacker != null)
            {
                _playerTarget = data.Attacker.transform;
            }

            if (!_isHurt)
            {
                StartCoroutine(HurtFlinchRoutine());
            }
        }

        private IEnumerator HurtFlinchRoutine()
        {
            _isHurt = true;
            MonsterAIState prevState = _currentState;
            _currentState = MonsterAIState.Hurt;

            yield return new WaitForSeconds(0.18f);

            _isHurt = false;
            // Transition straight to chase
            SetState(MonsterAIState.Chase);
        }

        private void HandleDeath()
        {
            SetState(MonsterAIState.Death);
        }
    }
}
