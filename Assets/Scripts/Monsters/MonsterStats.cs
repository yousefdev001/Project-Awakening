using System.Collections;
using Awakening.Combat;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// Runtime component holding monster stats, linking to HealthSystem and granting XP rewards.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Collider))]
    public class MonsterStats : MonoBehaviour
    {
        [Header("Data Blueprint")]
        [SerializeField] private MonsterData _data;

        public MonsterData Data => _data;
        public int Level => _data != null ? _data.level : 1;
        public string MonsterName => _data != null ? _data.monsterName : "Unknown Beast";
        public float AttackPower => _data != null ? _data.attackPower : 10f;
        public float Defense => _data != null ? _data.defense : 0f;
        public float MoveSpeed => _data != null ? _data.patrolSpeed : 2f;
        public float ChaseSpeed => _data != null ? _data.chaseSpeed : 4f;
        public float AttackRange => _data != null ? _data.attackRange : 1.8f;
        public float AttackCooldown => _data != null ? _data.attackCooldown : 1.5f;
        public float DetectionRadius => _data != null ? _data.detectionRadius : 8f;

        private HealthSystem _healthSystem;
        private Renderer _renderer;
        private Color _originalColor = Color.white;
        private bool _isDying = false;

        private void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            _renderer = GetComponentInChildren<Renderer>();

            if (_data == null)
            {
                _data = MonsterData.CreateSlimePreset();
            }

            ApplyMonsterData();
        }

        private void Start()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDamaged += HandleDamaged;
                _healthSystem.OnDeath += HandleDeath;
            }
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDamaged -= HandleDamaged;
                _healthSystem.OnDeath -= HandleDeath;
            }
        }

        public void SetMonsterData(MonsterData data)
        {
            _data = data;
            ApplyMonsterData();
        }

        private void ApplyMonsterData()
        {
            if (_data == null) return;

            transform.localScale = _data.modelScale;

            if (_renderer != null)
            {
                _renderer.material.color = _data.themeColor;
                _originalColor = _data.themeColor;
            }
        }

        private void HandleDamaged(DamageData data)
        {
            if (_isDying) return;
            StartCoroutine(FlashHurtRoutine());
        }

        private IEnumerator FlashHurtRoutine()
        {
            if (_renderer != null)
            {
                _renderer.material.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                if (!_isDying)
                {
                    _renderer.material.color = _originalColor;
                }
            }
        }

        private void HandleDeath()
        {
            if (_isDying) return;
            _isDying = true;

            Debug.Log($"<color=#FF5555>[Monster Defeated]</color> Defeated <b>[{_data.rank}] {_data.monsterName}</b>! Rewarding <b>+{_data.xpReward:F0} XP</b>.");

            // Award XP to player
            if (PlayerProgression.Instance != null)
            {
                PlayerProgression.Instance.AddXP(_data.xpReward);
            }

            StartCoroutine(DeathDisappearRoutine());
        }

        private IEnumerator DeathDisappearRoutine()
        {
            // Shrink and fade on death
            float duration = 1.0f;
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

        private void OnGUI()
        {
            if (_isDying || _healthSystem == null || Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * (_data != null ? _data.modelScale.y + 0.8f : 2.0f);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 25f)
            {
                float barW = 120;
                float barH = 14;
                float x = screenPos.x - (barW / 2);
                float y = Screen.height - screenPos.y;

                // Name & Level Tag
                string rankTag = _data.rank != MonsterRank.Normal ? $"<color=yellow>[{_data.rank}]</color> " : "";
                GUI.Label(new Rect(x - 20, y - 20, barW + 40, 20), $"<size=10><b>Lv. {_data.level} {rankTag}{_data.monsterName}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

                // Health Bar Background
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(x, y, barW, barH), Texture2D.whiteTexture);

                // Health Fill
                float hpPercent = _healthSystem.MaxHealth > 0 ? Mathf.Clamp01(_healthSystem.CurrentHealth / _healthSystem.MaxHealth) : 0f;
                GUI.color = new Color(0.9f, 0.2f, 0.2f);
                GUI.DrawTexture(new Rect(x + 1, y + 1, (barW - 2) * hpPercent, barH - 2), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.Label(new Rect(x, y - 1, barW, barH), $"<size=9>{_healthSystem.CurrentHealth:F0}/{_healthSystem.MaxHealth:F0}</size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
