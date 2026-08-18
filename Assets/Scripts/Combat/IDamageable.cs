using System;

namespace Awakening.Combat
{
    /// <summary>
    /// Contract for any entity in the game world that can take damage or be killed.
    /// </summary>
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }

        void TakeDamage(DamageData damageData);
        void Heal(float amount);

        event Action<DamageData> OnDamaged;
        event Action<float> OnHealed;
        event Action OnDeath;
    }
}
