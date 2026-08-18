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

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
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

            // Keyboard Space Jump fallback
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                HandleJump();
            }

            CheckGrounded();
            HandleGravity();
            HandleLocomotion();
        }

        private void CheckGrounded()
        {
            // Prefer CharacterController native grounded state
            if (_characterController != null && _characterController.isGrounded)
            {
                IsGrounded = true;
            }
            else
            {
                // Fallback sphere cast below character feet ignoring player's own layer
                Vector3 feetPos = transform.position + Vector3.down * (_characterController != null ? (_characterController.height * 0.5f - _characterController.radius) : 0.8f);
                Collider[] hits = Physics.OverlapSphere(feetPos, _config.groundCheckRadius, _config.groundLayers, QueryTriggerInteraction.Ignore);
                
                bool foundSolidGround = false;
                foreach (var hit in hits)
                {
                    if (hit.gameObject != gameObject && !hit.transform.IsChildOf(transform))
                    {
                        foundSolidGround = true;
                        break;
                    }
                }
                IsGrounded = foundSolidGround;
            }

            // Clamp vertical velocity only when falling/grounded (never cancel jump upward burst)
            if (IsGrounded && _verticalVelocity < 0.0f)
            {
                _verticalVelocity = _config.groundedDownwardForce;
            }
        }

        private void HandleGravity()
        {
            // Apply gravity whenever in air OR moving upward in jump arc
            if (!IsGrounded || _verticalVelocity > 0.0f)
            {
                if (_verticalVelocity > _config.terminalVelocity)
                {
                    _verticalVelocity += _config.gravity * Time.deltaTime;
                }
            }
        }

        private void HandleLocomotion()
        {
            Vector2 input = Vector2.zero;
            if (_inputProvider != null)
            {
                input = _inputProvider.MoveInput;
            }

            // Direct keyboard fallback for 100% reliable WASD response
            if (input.sqrMagnitude < 0.001f && UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.wKey.isPressed) input.y += 1;
                if (kb.sKey.isPressed) input.y -= 1;
                if (kb.aKey.isPressed) input.x -= 1;
                if (kb.dKey.isPressed) input.x += 1;
                input = input.normalized;
            }

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
                bool isSprinting = (_inputProvider != null && _inputProvider.IsSprinting) ||
                                  (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed);

                float baseLocomotionSpeed = isSprinting ? _config.sprintSpeed : _config.walkSpeed;
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
