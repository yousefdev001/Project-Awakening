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

            // Target movement speed
            float targetSpeed = 0f;
            if (input.sqrMagnitude > 0.01f)
            {
                targetSpeed = _inputProvider.IsSprinting ? _config.sprintSpeed : _config.walkSpeed;
            }

            // Smooth acceleration and deceleration
            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = Mathf.Clamp01(input.magnitude);

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _config.speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed * inputMagnitude;
            }

            CurrentSpeed = _speed;

            Vector3 moveDirection = Vector3.zero;

            // Rotate towards direction of movement relative to camera
            if (input.sqrMagnitude > 0.01f)
            {
                Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;

                float cameraYaw = _cameraTransform != null ? _cameraTransform.eulerAngles.y : 0f;
                _targetRotationAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraYaw;

                float smoothAngle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotationAngle,
                    ref _rotationVelocity,
                    _config.rotationSmoothTime
                );

                transform.rotation = Quaternion.Euler(0.0f, smoothAngle, 0.0f);

                // Movement vector relative to the target angle
                moveDirection = Quaternion.Euler(0.0f, _targetRotationAngle, 0.0f) * Vector3.forward;
            }

            // Final motion: Horizontal locomotion + Vertical gravity/jump
            Vector3 finalMotion = (moveDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime;
            _characterController.Move(finalMotion);

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
