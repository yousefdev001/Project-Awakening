using Awakening.Core;
using Awakening.Input;
using UnityEngine;

namespace Awakening.Player
{
    /// <summary>
    /// Smooth orbital Third-Person Camera with collision avoidance and pitch clamping.
    /// Follows the player target and responds to mouse look inputs.
    /// </summary>
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target & Configuration")]
        [SerializeField] private Transform _target;
        [SerializeField] private CameraConfig _config;

        [Header("Cursor Settings")]
        [SerializeField] private bool _lockCursorOnStart = true;

        private IInputProvider _inputProvider;
        private float _currentYaw;
        private float _currentPitch = 15.0f;
        private float _targetYaw;
        private float _targetPitch = 15.0f;
        private float _currentDistance;

        private Vector3 _currentFocusPoint;

        private void Awake()
        {
            if (_config == null)
            {
                _config = ScriptableObject.CreateInstance<CameraConfig>();
            }

            _currentDistance = _config.defaultDistance;

            // Initialize yaw and pitch from current camera orientation
            Vector3 angles = transform.eulerAngles;
            _targetYaw = _currentYaw = angles.y;
            _targetPitch = _currentPitch = angles.x;
        }

        private void Start()
        {
            _inputProvider = InputReader.Instance;

            if (_target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _target = playerObj.transform;
                }
                else
                {
                    PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
                    if (movement != null)
                    {
                        _target = movement.transform;
                    }
                }
            }

            if (_target != null)
            {
                _currentFocusPoint = _target.position + _config.targetOffset;
            }

            // Keep cursor visible and unlocked for RPG interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                return;
            }

            // Only rotate camera when inventory is closed
            bool isInventoryOpen = Inventory.InventorySystem.Instance != null && Inventory.InventorySystem.Instance.IsOpen;
            if (!isInventoryOpen)
            {
                HandleInput();
            }

            UpdateCameraTransform();
        }

        private void HandleInput()
        {
            Vector2 look = _inputProvider != null ? _inputProvider.LookInput : Vector2.zero;

            if (look.sqrMagnitude > 0.001f)
            {
                _targetYaw += look.x * _config.mouseSensitivity;
                _targetPitch -= look.y * _config.mouseSensitivity;
                _targetPitch = Mathf.Clamp(_targetPitch, _config.minPitch, _config.maxPitch);
            }

            // Unlock cursor on Esc click if desired
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                LockCursor(Cursor.lockState != CursorLockMode.Locked);
            }
        }

        private void UpdateCameraTransform()
        {
            // Smoothly interpolate yaw and pitch
            _currentYaw = Mathf.Lerp(_currentYaw, _targetYaw, Time.deltaTime * _config.rotationSharpness);
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, Time.deltaTime * _config.rotationSharpness);

            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);

            // Target focus position (Player head/chest)
            Vector3 targetFocus = _target.position + _config.targetOffset;
            _currentFocusPoint = Vector3.Lerp(_currentFocusPoint, targetFocus, Time.deltaTime * _config.followSharpness);

            // Desired camera position before collision check
            Vector3 direction = rotation * Vector3.back;
            float desiredDistance = _config.defaultDistance;

            // SphereCast collision check from focus point towards camera
            if (Physics.SphereCast(
                _currentFocusPoint,
                _config.collisionRadius,
                direction,
                out RaycastHit hit,
                desiredDistance,
                _config.collisionLayers,
                QueryTriggerInteraction.Ignore))
            {
                // Pull camera forward in front of the obstacle
                desiredDistance = Mathf.Clamp(hit.distance - _config.collisionOffset, _config.minDistance, _config.maxDistance);
            }

            _currentDistance = Mathf.Lerp(_currentDistance, desiredDistance, Time.deltaTime * 20.0f);

            Vector3 finalPosition = _currentFocusPoint + direction * _currentDistance;

            transform.position = finalPosition;
            transform.rotation = rotation;
        }

        private void OnDrawGizmosSelected()
        {
            if (_target == null || _config == null) return;

            Vector3 focus = _target.position + _config.targetOffset;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(focus, 0.2f);
            Gizmos.DrawLine(focus, transform.position);
        }
    }
}
