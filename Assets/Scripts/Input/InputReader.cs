using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Awakening.Input
{
    /// <summary>
    /// Core Input Reader component that bridges Unity's New Input System with game systems.
    /// Implements IInputProvider to provide decoupled input access.
    /// </summary>
    public class InputReader : MonoBehaviour, IInputProvider
    {
        public static InputReader Instance { get; private set; }

        [Header("Runtime Values (Read Only)")]
        [SerializeField] private Vector2 _moveInput;
        [SerializeField] private Vector2 _lookInput;
        [SerializeField] private bool _isSprinting;

        public Vector2 MoveInput => _moveInput;
        public Vector2 LookInput => _lookInput;
        public bool IsSprinting => _isSprinting;

        public event Action OnJump;
        public event Action OnDodge;
        public event Action OnAttack;
        public event Action OnHeavyAttack;
        public event Action OnSkill;
        public event Action OnInteract;
        public event Action OnInventoryToggle;
        public event Action OnPauseToggle;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _sprintAction;
        private InputAction _jumpAction;
        private InputAction _dodgeAction;
        private InputAction _attackAction;
        private InputAction _heavyAttackAction;
        private InputAction _skillAction;
        private InputAction _interactAction;
        private InputAction _inventoryAction;
        private InputAction _pauseAction;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeInputActions();
        }

        private void OnEnable()
        {
            EnableAllActions();
        }

        private void OnDisable()
        {
            DisableAllActions();
        }

        private void InitializeInputActions()
        {
            // Move: 2D Vector composite (WASD / Left Stick)
            _moveAction = new InputAction("Move", InputActionType.Value);
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            _moveAction.AddBinding("<Gamepad>/leftStick");

            // Look: 2D Vector (Mouse Delta / Right Stick)
            _lookAction = new InputAction("Look", InputActionType.Value);
            _lookAction.AddBinding("<Mouse>/delta");
            _lookAction.AddBinding("<Gamepad>/rightStick");

            // Sprint: Button / Hold (Left Shift / Gamepad Left Stick Press)
            _sprintAction = new InputAction("Sprint", InputActionType.Button);
            _sprintAction.AddBinding("<Keyboard>/leftShift");
            _sprintAction.AddBinding("<Gamepad>/leftStickPress");

            // Jump: Button (Space / Gamepad South)
            _jumpAction = new InputAction("Jump", InputActionType.Button);
            _jumpAction.AddBinding("<Keyboard>/space");
            _jumpAction.AddBinding("<Gamepad>/buttonSouth");

            // Dodge: Button (Left Alt, C / Gamepad East)
            _dodgeAction = new InputAction("Dodge", InputActionType.Button);
            _dodgeAction.AddBinding("<Keyboard>/leftAlt");
            _dodgeAction.AddBinding("<Keyboard>/c");
            _dodgeAction.AddBinding("<Gamepad>/buttonEast");

            // Attack (Light): Button (Left Mouse / Gamepad West)
            _attackAction = new InputAction("Attack", InputActionType.Button);
            _attackAction.AddBinding("<Mouse>/leftButton");
            _attackAction.AddBinding("<Gamepad>/buttonWest");

            // Heavy Attack: Button (Right Mouse / Gamepad North)
            _heavyAttackAction = new InputAction("HeavyAttack", InputActionType.Button);
            _heavyAttackAction.AddBinding("<Mouse>/rightButton");
            _heavyAttackAction.AddBinding("<Gamepad>/buttonNorth");

            // Skill: Button (E / Gamepad Right Shoulder)
            _skillAction = new InputAction("Skill", InputActionType.Button);
            _skillAction.AddBinding("<Keyboard>/e");
            _skillAction.AddBinding("<Gamepad>/rightShoulder");

            // Interact: Button (F / Gamepad X/West secondary)
            _interactAction = new InputAction("Interact", InputActionType.Button);
            _interactAction.AddBinding("<Keyboard>/f");
            _interactAction.AddBinding("<Gamepad>/dpad/up");

            // Inventory: Button (Tab, I / Gamepad Select)
            _inventoryAction = new InputAction("Inventory", InputActionType.Button);
            _inventoryAction.AddBinding("<Keyboard>/tab");
            _inventoryAction.AddBinding("<Keyboard>/i");
            _inventoryAction.AddBinding("<Gamepad>/select");

            // Pause: Button (Escape / Gamepad Start)
            _pauseAction = new InputAction("Pause", InputActionType.Button);
            _pauseAction.AddBinding("<Keyboard>/escape");
            _pauseAction.AddBinding("<Gamepad>/start");

            // Hook Callbacks
            _moveAction.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            _moveAction.canceled += _ => _moveInput = Vector2.zero;

            _lookAction.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
            _lookAction.canceled += _ => _lookInput = Vector2.zero;

            _sprintAction.performed += _ => _isSprinting = true;
            _sprintAction.canceled += _ => _isSprinting = false;

            _jumpAction.performed += _ => OnJump?.Invoke();
            _dodgeAction.performed += _ => OnDodge?.Invoke();
            _attackAction.performed += _ => OnAttack?.Invoke();
            _heavyAttackAction.performed += _ => OnHeavyAttack?.Invoke();
            _skillAction.performed += _ => OnSkill?.Invoke();
            _interactAction.performed += _ => OnInteract?.Invoke();
            _inventoryAction.performed += _ => OnInventoryToggle?.Invoke();
            _pauseAction.performed += _ => OnPauseToggle?.Invoke();
        }

        private void EnableAllActions()
        {
            _moveAction?.Enable();
            _lookAction?.Enable();
            _sprintAction?.Enable();
            _jumpAction?.Enable();
            _dodgeAction?.Enable();
            _attackAction?.Enable();
            _heavyAttackAction?.Enable();
            _skillAction?.Enable();
            _interactAction?.Enable();
            _inventoryAction?.Enable();
            _pauseAction?.Enable();
        }

        private void DisableAllActions()
        {
            _moveAction?.Disable();
            _lookAction?.Disable();
            _sprintAction?.Disable();
            _jumpAction?.Disable();
            _dodgeAction?.Disable();
            _attackAction?.Disable();
            _heavyAttackAction?.Disable();
            _skillAction?.Disable();
            _interactAction?.Disable();
            _inventoryAction?.Disable();
            _pauseAction?.Disable();
        }

        private void OnDestroy()
        {
            _moveAction?.Dispose();
            _lookAction?.Dispose();
            _sprintAction?.Dispose();
            _jumpAction?.Dispose();
            _dodgeAction?.Dispose();
            _attackAction?.Dispose();
            _heavyAttackAction?.Dispose();
            _skillAction?.Dispose();
            _interactAction?.Dispose();
            _inventoryAction?.Dispose();
            _pauseAction?.Dispose();
        }
    }
}
