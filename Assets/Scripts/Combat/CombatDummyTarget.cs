using System.Collections;
using UnityEngine;

namespace Awakening.Combat
{
    /// <summary>
    /// Training dummy target to test attacks, damage numbers, and knockback in Play Mode.
    /// Automatically flashes red on hit and resets health on death.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Collider))]
    public class CombatDummyTarget : MonoBehaviour
    {
        [SerializeField] private float _respawnTime = 3.0f;

        private HealthSystem _healthSystem;
        private Renderer _renderer;
        private Color _originalColor = Color.white;
        private Vector3 _spawnPosition;

        private void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            _renderer = GetComponentInChildren<Renderer>();
            _spawnPosition = transform.position;

            if (_renderer != null && _renderer.material != null)
            {
                _originalColor = _renderer.material.color;
            }
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

        private void HandleDamaged(DamageData data)
        {
            StartCoroutine(FlashRedRoutine());

            // Apply simple knockback if rigid body or transform
            if (data.KnockbackForce > 0f)
            {
                Vector3 knockbackDir = (transform.position - data.HitPoint).normalized;
                knockbackDir.y = 0f;
                transform.position += knockbackDir * (data.KnockbackForce * 0.15f);
            }
        }

        private IEnumerator FlashRedRoutine()
        {
            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.color = Color.red;
                yield return new WaitForSeconds(0.12f);
                _renderer.material.color = _originalColor;
            }
        }

        private void HandleDeath()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            Debug.Log($"<color=#FFFF00>[DummyTarget]</color> {gameObject.name} was defeated! Respawning in {_respawnTime}s...");
            yield return new WaitForSeconds(_respawnTime);

            transform.position = _spawnPosition;
            if (_healthSystem != null)
            {
                _healthSystem.Heal(_healthSystem.MaxHealth);
            }
        }

        private void OnGUI()
        {
            if (_healthSystem == null || Camera.main == null) return;

            // Draw floating health label above dummy
            Vector3 worldPos = transform.position + Vector3.up * 2.2f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                float barW = 80;
                float barH = 16;
                float x = screenPos.x - (barW / 2);
                float y = Screen.height - screenPos.y;

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(x, y, barW, barH), Texture2D.whiteTexture);

                float hpPercent = _healthSystem.MaxHealth > 0 ? Mathf.Clamp01(_healthSystem.CurrentHealth / _healthSystem.MaxHealth) : 0f;
                GUI.color = Color.red;
                GUI.DrawTexture(new Rect(x + 1, y + 1, (barW - 2) * hpPercent, barH - 2), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.Label(new Rect(x, y - 18, barW, 20), $"Dummy HP: {_healthSystem.CurrentHealth:F0}", new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
