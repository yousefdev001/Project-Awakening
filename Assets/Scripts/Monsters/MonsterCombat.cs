using Awakening.Combat;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// Handles monster attack execution, melee strike detection against the player, and cooldowns.
    /// </summary>
    [RequireComponent(typeof(MonsterStats))]
    public class MonsterCombat : MonoBehaviour
    {
        private MonsterStats _stats;
        private float _lastAttackTime = -99f;
        private Transform _playerTransform;

        private void Awake()
        {
            _stats = GetComponent<MonsterStats>();
        }

        private void Start()
        {
            FindPlayerTarget();
        }

        private void FindPlayerTarget()
        {
            if (PlayerMovement.FindFirstObjectByType<PlayerMovement>() != null)
            {
                _playerTransform = PlayerMovement.FindFirstObjectByType<PlayerMovement>().transform;
            }
        }

        public bool TryAttackTarget(IDamageable target)
        {
            if (target == null || target.IsDead) return false;
            if (Time.time < _lastAttackTime + _stats.AttackCooldown) return false;

            _lastAttackTime = Time.time;

            DamageData data = new DamageData(
                amount: _stats.AttackPower,
                damageType: DamageType.Physical,
                attacker: gameObject,
                hitPoint: transform.position + transform.forward * _stats.AttackRange,
                knockbackForce: 3.0f
            );

            target.TakeDamage(data);

            Debug.Log($"<color=#FF4444>[Monster Attack]</color> <b>{_stats.MonsterName}</b> struck player for <b>{_stats.AttackPower:F1}</b> dmg!");
            return true;
        }

        public bool IsTargetInAttackRange(Vector3 targetPosition)
        {
            float sqrDist = (transform.position - targetPosition).sqrMagnitude;
            return sqrDist <= (_stats.AttackRange * _stats.AttackRange);
        }
    }
}
