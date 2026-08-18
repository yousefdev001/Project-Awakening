using Awakening.Core;
using Awakening.Input;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Core Player Movement controller using CharacterController.
    /// Handles camera-relative movement, smooth rotation, jumping, and gravity.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PlayerConfig _config;

        [Header("References")]
        [SerializeField] private Transform _cameraTransform;

        // Runtime State
        public float CurrentSpeed { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsSprinting => _inputProvider != null && _inputProvider.IsSprinting && _inputProvider.MoveInput.sqrMagnitude > 0.01f;
        public Vector3 HorizontalVelocity { get; private set; }

        private CharacterController _characterController;
        private IInputProvider _inputProvider;

        private float _targetRotationAngle;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _currentSpeedVelocity;
        private float _speed;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            // Default fallback config if none assigned
            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<PlayerConfig>();
            }
        }

        private void Start()
        {
            // Acquire InputProvider (from this GameObject or the global InputReader instance)
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;

            if (_inputProvider != null)
            {
                _inputProvider.OnJump += HandleJump;
            }
            else
            {
                Debug.LogWarning("[PlayerMovement] No IInputProvider found. Make sure InputReader exists in the scene.");
            }

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnJump -= HandleJump;
            }
        }

        private void Update()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                return;
            }

            // If Inventory or Dialogue is open, apply gravity only and skip locomotion
            bool isBusyWithUI = (Inventory.InventorySystem.Instance != null && Inventory.InventorySystem.Instance.IsOpen)
                || (GameUI.DialogueUI.Instance != null && GameUI.DialogueUI.Instance.IsInDialogue);

            if (isBusyWithUI)
            {
                CheckGrounded();
                HandleGravity();
                return;
            }

            CheckGrounded();
            HandleGravity();
            HandleLocomotion();
        }

        private void CheckGrounded()
        {
            // Position check sphere at bottom center of the CharacterController
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - _config.groundCheckOffset,
                transform.position.z
            );

            IsGrounded = Physics.CheckSphere(
                spherePosition,
                _config.groundCheckRadius,
                _config.groundLayers,
                QueryTriggerInteraction.Ignore
            );

            if (IsGrounded && _verticalVelocity < 0.0f)
            {
                _verticalVelocity = _config.groundedDownwardForce;
            }
        }

        private void HandleGravity()
        {
            if (!IsGrounded)
            {
                if (_verticalVelocity > _config.terminalVelocity)
                {
                    _verticalVelocity += _config.gravity * Time.deltaTime;
                }
            }
        }

        private void HandleLocomotion()
        {
            Vector2 input = _inputProvider != null ? _inputProvider.MoveInput : Vector2.zero;

            // Calculate stat speed scaling
            float speedScale = 1.0f;
            if (PlayerStats.Instance != null && _config.baseSpeed > 0.01f)
            {
                speedScale = PlayerStats.Instance.Speed / _config.baseSpeed;
            }

            // Target movement speed and direction
            float targetSpeed = 0f;
            Vector3 moveDirection = Vector3.zero;

            if (input.sqrMagnitude > 0.01f)
            {
                float baseLocomotionSpeed = _inputProvider.IsSprinting ? _config.sprintSpeed : _config.walkSpeed;
                targetSpeed = baseLocomotionSpeed * speedScale;

                // Calculate direction relative to camera
                Vector3 forward = _cameraTransform != null ? _cameraTransform.forward : Vector3.forward;
                Vector3 right = _cameraTransform != null ? _cameraTransform.right : Vector3.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                moveDirection = (forward * input.y + right * input.x).normalized;

                // Smoothly rotate character towards movement direction
                if (moveDirection.sqrMagnitude > 0.001f)
                {
                    float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                    float smoothAngle = Mathf.SmoothDampAngle(
                        transform.eulerAngles.y,
                        targetAngle,
                        ref _rotationVelocity,
                        _config.rotationSmoothTime
                    );
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                }
            }

            // Smooth speed transitions with MoveTowards to prevent any sudden spikes
            _speed = Mathf.MoveTowards(_speed, targetSpeed, _config.speedChangeRate * 2.0f * Time.deltaTime);
            CurrentSpeed = _speed;

            // Final motion: Horizontal locomotion + Vertical gravity/jump
            Vector3 motion = (moveDirection * _speed) + (Vector3.up * _verticalVelocity);
            _characterController.Move(motion * Time.deltaTime);

            HorizontalVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        }

        private void HandleJump()
        {
            if (IsGrounded)
            {
                // v = sqrt(h * -2 * g)
                _verticalVelocity = Mathf.Sqrt(_config.jumpHeight * -2.0f * _config.gravity);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_config == null) return;

            // Draw Ground Check Sphere in Editor
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - _config.groundCheckOffset,
                transform.position.z
            );
            Gizmos.DrawWireSphere(spherePosition, _config.groundCheckRadius);
        }
    }
}
