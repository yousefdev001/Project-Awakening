using System;
using Awakening.Input;
using UnityEngine;

namespace Awakening.Core
{
    /// <summary>
    /// Manages the top-level GameState machine and handles time-scale / pause transitions.
    /// Ensures mouse cursor remains visible and interactive across gameplay and UI screens.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [Header("Initial State")]
        [SerializeField] private GameState _initialState = GameState.MainMenu;

        public GameState CurrentState { get; private set; }
        public GameState PreviousState { get; private set; }

        public event Action<GameState, GameState> OnGameStateChanged;
        public event Action OnGamePaused;
        public event Action OnGameResumed;

        private IInputProvider _inputProvider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CurrentState = _initialState;
            PreviousState = _initialState;

            // Auto-mount master GameBootstrap if not present
            if (GetComponent<GameBootstrap>() == null)
            {
                gameObject.AddComponent<GameBootstrap>();
            }
        }

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;

            if (_inputProvider != null)
            {
                _inputProvider.OnPauseToggle += HandlePauseToggle;
            }

            ApplyStateEffects(CurrentState);
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnPauseToggle -= HandlePauseToggle;
            }
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            PreviousState = CurrentState;
            CurrentState = newState;

            ApplyStateEffects(newState);

            OnGameStateChanged?.Invoke(PreviousState, CurrentState);

            if (CurrentState == GameState.Paused)
            {
                OnGamePaused?.Invoke();
            }
            else if (PreviousState == GameState.Paused && CurrentState == GameState.Gameplay)
            {
                OnGameResumed?.Invoke();
            }

            Debug.Log($"<color=#00D4FF>[GameState]</color> Transitioned: <b>{PreviousState}</b> ➔ <b>{CurrentState}</b>");
        }

        private void HandlePauseToggle()
        {
            if (CurrentState == GameState.Gameplay)
            {
                SetState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Gameplay);
            }
        }

        private void ApplyStateEffects(GameState state)
        {
            // Keep cursor always visible and unlocked for smooth UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            switch (state)
            {
                case GameState.Gameplay:
                    Time.timeScale = 1.0f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    break;

                case GameState.MainMenu:
                case GameState.CharacterCreation:
                case GameState.GameOver:
                case GameState.Awakening:
                    Time.timeScale = 1.0f;
                    break;
            }
        }
    }
}
