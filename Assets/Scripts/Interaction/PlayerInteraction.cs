using System;
using Awakening.Core;
using Awakening.Input;
using Awakening.Inventory;
using UnityEngine;

namespace Awakening.Interaction
{
    /// <summary>
    /// Player component that detects nearby IInteractable objects and triggers interaction on [F] key press.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        public static PlayerInteraction Instance { get; private set; }

        [Header("Detection Settings")]
        [SerializeField] private float _detectionRadius = 2.8f;
        [SerializeField] private LayerMask _interactableLayers = ~0;

        public IInteractable CurrentInteractable { get; private set; }
        public bool HasInteractable => CurrentInteractable != null;

        public event Action<IInteractable> OnInteractableFocused;
        public event Action<IInteractable> OnInteractionExecuted;

        private IInputProvider _inputProvider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;
            if (_inputProvider != null)
            {
                _inputProvider.OnInteract += HandleInteract;
            }
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
            {
                _inputProvider.OnInteract -= HandleInteract;
            }
        }

        private void Update()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay)
            {
                ClearCurrentInteractable();
                return;
            }

            if (InventorySystem.Instance != null && InventorySystem.Instance.IsOpen)
            {
                ClearCurrentInteractable();
                return;
            }

            ScanForInteractables();
        }

        private void ScanForInteractables()
        {
            Vector3 checkCenter = transform.position + Vector3.up * 0.5f;
            Collider[] hits = Physics.OverlapSphere(checkCenter, _detectionRadius, _interactableLayers, QueryTriggerInteraction.Collide);

            IInteractable bestTarget = null;
            float closestDistSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                // Skip self
                if (hit.gameObject == gameObject || hit.transform.IsChildOf(transform)) continue;

                IInteractable interactable = hit.GetComponentInParent<IInteractable>() ?? hit.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(gameObject))
                {
                    float distSqr = (hit.transform.position - transform.position).sqrMagnitude;
                    if (distSqr < closestDistSqr)
                    {
                        closestDistSqr = distSqr;
                        bestTarget = interactable;
                    }
                }
            }

            if (bestTarget != CurrentInteractable)
            {
                CurrentInteractable = bestTarget;
                OnInteractableFocused?.Invoke(CurrentInteractable);
            }
        }

        private void HandleInteract()
        {
            if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState != GameState.Gameplay) return;
            if (InventorySystem.Instance != null && InventorySystem.Instance.IsOpen) return;

            if (CurrentInteractable != null && CurrentInteractable.CanInteract(gameObject))
            {
                IInteractable target = CurrentInteractable;
                Debug.Log($"<color=#00FFAA>[Interaction]</color> Interacting with: <b>{target.InteractionPrompt}</b>");
                target.Interact(gameObject);
                OnInteractionExecuted?.Invoke(target);
            }
        }

        private void ClearCurrentInteractable()
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable = null;
                OnInteractableFocused?.Invoke(null);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0.8f, 0.3f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, _detectionRadius);
        }
    }
}
