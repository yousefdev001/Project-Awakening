using UnityEngine;

namespace Awakening.Combat
{
    /// <summary>
    /// Encapsulates contextual information for an attack or damage event.
    /// </summary>
    public struct DamageData
    {
        public float Amount;
        public DamageType DamageType;
        public GameObject Attacker;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public bool IsCritical;
        public float KnockbackForce;

        public DamageData(
            float amount,
            DamageType damageType = DamageType.Physical,
            GameObject attacker = null,
            Vector3 hitPoint = default,
            Vector3 hitNormal = default,
            bool isCritical = false,
            float knockbackForce = 0f)
        {
            Amount = amount;
            DamageType = damageType;
            Attacker = attacker;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            IsCritical = isCritical;
            KnockbackForce = knockbackForce;
        }
    }
}
