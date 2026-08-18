using Awakening.Input;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Connects PlayerMovement physics and InputReader events to the Unity Animator.
    /// Manages speed blend values, jump/fall states, and combat triggers.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("Animator Reference")]
        [SerializeField] private Animator _animator;

        [Header("Damping Settings")]
        [SerializeField] private float _speedDampTime = 0.1f;

        // Cached Animator Parameter Hashes for zero GC performance
        public static readonly int SpeedHash = Animator.StringToHash("Speed");
        public static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        public static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
        public static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
        public static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        public static readonly int HeavyAttackTriggerHash = Animator.StringToHash("HeavyAttack");
        public static readonly int DodgeTriggerHash = Animator.StringToHash("Dodge");
        public static readonly int HitTriggerHash = Animator.StringToHash("Hit");
        public static readonly int DieTriggerHash = Animator.StringToHash("Die");

        private PlayerMovement _movement;
        private CharacterController _characterController;
        private IInputProvider _inputProvider;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _characterController = GetComponent<CharacterController>();

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            if (_animator != null)
            {
                _animator.applyRootMotion = false;
            }
        }

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;

            if (_inputProvider != null)
            {
                _inputProvider.OnJump += TriggerJumpAnimation;
            }
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnJump -= TriggerJumpAnimation;
            }
        }

        private void Update()
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
                if (_animator == null) return;
            }

            UpdateLocomotionParameters();
        }

        private void UpdateLocomotionParameters()
        {
            // Calculate normalized speed: 0 = Idle, 1 = Walk, 2 = Sprint
            float targetSpeedValue = 0f;
            if (_movement.CurrentSpeed > 0.1f)
            {
                targetSpeedValue = _movement.IsSprinting ? 2.0f : 1.0f;
            }

            _animator.SetFloat(SpeedHash, targetSpeedValue, _speedDampTime, Time.deltaTime);
            _animator.SetBool(IsGroundedHash, _movement.IsGrounded);

            // Is falling: in the air and moving downwards
            bool isFalling = !_movement.IsGrounded && _characterController.velocity.y < -1.5f;
            _animator.SetBool(IsFallingHash, isFalling);
        }

        public void TriggerJumpAnimation()
        {
            if (Core.GameStateManager.Instance != null && Core.GameStateManager.Instance.CurrentState != Core.GameState.Gameplay) return;
            if (Inventory.InventorySystem.Instance != null && Inventory.InventorySystem.Instance.IsOpen) return;

            if (_animator != null && _movement.IsGrounded)
            {
                _animator.SetTrigger(JumpTriggerHash);
            }
        }

        public void TriggerAttackAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(AttackTriggerHash);
            }
        }

        public void TriggerHeavyAttackAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(HeavyAttackTriggerHash);
            }
        }

        public void TriggerDodgeAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(DodgeTriggerHash);
            }
        }

        public void TriggerHitAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(HitTriggerHash);
            }
        }

        public void TriggerDieAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(DieTriggerHash);
            }
        }
    }
}
