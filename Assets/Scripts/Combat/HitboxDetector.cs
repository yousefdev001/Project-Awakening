using System.Collections.Generic;
using UnityEngine;

namespace Awakening.Combat
{
    /// <summary>
    /// Utility detector to query and damage IDamageable entities within an attack area.
    /// </summary>
    public class HitboxDetector : MonoBehaviour
    {
        [Header("Hitbox Geometry")]
        [SerializeField] private float _attackRadius = 1.8f;
        [SerializeField] private float _forwardOffset = 1.2f;
        [SerializeField] private float _verticalOffset = 1.0f;

        [Header("Filtering")]
        [SerializeField] private LayerMask _targetLayers = ~0;

        public List<IDamageable> DetectAndDamageTargets(DamageData damageData)
        {
            List<IDamageable> hitEntities = new List<IDamageable>();

            Vector3 hitCenter = transform.position 
                + (transform.forward * _forwardOffset) 
                + (Vector3.up * _verticalOffset);

            Collider[] hits = Physics.OverlapSphere(hitCenter, _attackRadius, _targetLayers, QueryTriggerInteraction.Ignore);

            foreach (Collider col in hits)
            {
                // Prevent damaging self
                if (col.gameObject == gameObject || col.transform.IsChildOf(transform) || transform.IsChildOf(col.transform))
                {
                    continue;
                }

                IDamageable damageable = col.GetComponentInParent<IDamageable>();
                if (damageable != null && !damageable.IsDead && !hitEntities.Contains(damageable))
                {
                    // Calculate hit point
                    Vector3 contactPoint = col.ClosestPoint(hitCenter);
                    damageData.HitPoint = contactPoint;
                    damageData.HitNormal = (contactPoint - hitCenter).normalized;

                    damageable.TakeDamage(damageData);
                    hitEntities.Add(damageable);
                }
            }

            return hitEntities;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
            Vector3 hitCenter = transform.position 
                + (transform.forward * _forwardOffset) 
                + (Vector3.up * _verticalOffset);
            Gizmos.DrawWireSphere(hitCenter, _attackRadius);
        }
    }
}
