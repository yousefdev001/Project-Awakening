using System;
using UnityEngine;

namespace Awakening.Input
{
    /// <summary>
    /// Contract for reading player input across all gameplay systems.
    /// Decouples movement, combat, and UI logic from specific hardware inputs.
    /// </summary>
    public interface IInputProvider
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool IsSprinting { get; }

        event Action OnJump;
        event Action OnDodge;
        event Action OnAttack;
        event Action OnHeavyAttack;
        event Action OnSkill;
        event Action OnInteract;
        event Action OnInventoryToggle;
        event Action OnPauseToggle;
    }
}
