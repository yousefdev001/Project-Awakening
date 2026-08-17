using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Configuration data for player locomotion and physics.
    /// ScriptableObject allows runtime tuning without touching scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Awakening/Player/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Locomotion Speeds")]
        [Tooltip("Standard walking speed in m/s")]
        public float walkSpeed = 4.5f;

        [Tooltip("Sprint speed in m/s when holding sprint")]
        public float sprintSpeed = 8.0f;

        [Tooltip("Smooth time for speed transitions")]
        public float speedChangeRate = 10.0f;

        [Header("Rotation")]
        [Tooltip("Smooth time for character facing movement direction")]
        public float rotationSmoothTime = 0.12f;

        [Header("Jumping & Gravity")]
        [Tooltip("Peak jump height in meters")]
        public float jumpHeight = 1.5f;

        [Tooltip("Gravity acceleration in m/s^2")]
        public float gravity = -22.0f;

        [Tooltip("Terminal fall velocity")]
        public float terminalVelocity = -53.0f;

        [Tooltip("Small downward force when grounded to stay glued to slopes/stairs")]
        public float groundedDownwardForce = -2.0f;

        [Header("Ground Detection")]
        [Tooltip("Radius of the ground check sphere")]
        public float groundCheckRadius = 0.28f;

        [Tooltip("Vertical offset from character base for ground check")]
        public float groundCheckOffset = 0.15f;

        [Tooltip("Layers considered solid ground")]
        public LayerMask groundLayers = ~0; // Default to everything except player
    }
}
