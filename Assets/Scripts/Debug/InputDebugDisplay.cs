using System;
using Awakening.Input;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug component that visualizes real-time input status on screen (OnGUI) and in the Console.
    /// Useful for Phase 1 verification and QA.
    /// </summary>
    public class InputDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showOnScreenGUI = true;
        [SerializeField] private bool _logEventsToConsole = true;

        private IInputProvider _inputProvider;
        private string _lastAction = "None";
        private float _lastActionTime = 0f;

        private void Start()
        {
            _inputProvider = GetComponent<IInputProvider>() ?? InputReader.Instance;

            if (_inputProvider != null)
            {
                _inputProvider.OnJump += () => RecordAction("Jump (Space)");
                _inputProvider.OnDodge += () => RecordAction("Dodge (Alt/C)");
                _inputProvider.OnAttack += () => RecordAction("Light Attack (LMB)");
                _inputProvider.OnHeavyAttack += () => RecordAction("Heavy Attack (RMB)");
                _inputProvider.OnSkill += () => RecordAction("Skill (E)");
                _inputProvider.OnInteract += () => RecordAction("Interact (F)");
                _inputProvider.OnInventoryToggle += () => RecordAction("Toggle Inventory (Tab/I)");
                _inputProvider.OnPauseToggle += () => RecordAction("Toggle Pause (Esc)");
            }
            else
            {
                Debug.LogWarning("[InputDebugDisplay] No IInputProvider found. Make sure InputReader is in the scene.");
            }
        }

        private void RecordAction(string actionName)
        {
            _lastAction = actionName;
            _lastActionTime = Time.time;

            if (_logEventsToConsole)
            {
                Debug.Log($"<color=#00FFAA>[Input Event]</color> Triggered: <b>{actionName}</b> at {Time.time:F2}s");
            }
        }

        private void OnGUI()
        {
            if (!_showOnScreenGUI) return;

            GUI.Box(new Rect(10, 10, 320, 220), "🎮 Input System Debug (Phase 1)");

            if (_inputProvider == null)
            {
                GUI.Label(new Rect(20, 40, 300, 30), "❌ InputReader not found!");
                return;
            }

            Vector2 move = _inputProvider.MoveInput;
            Vector2 look = _inputProvider.LookInput;
            bool sprinting = _inputProvider.IsSprinting;

            GUI.Label(new Rect(20, 35, 300, 25), $"Movement (WASD): ({move.x:F2}, {move.y:F2})");
            GUI.Label(new Rect(20, 60, 300, 25), $"Look (Mouse Delta): ({look.x:F2}, {look.y:F2})");
            GUI.Label(new Rect(20, 85, 300, 25), $"Sprinting (Shift): {(sprinting ? "<color=green>YES</color>" : "NO")}");
            GUI.Label(new Rect(20, 110, 300, 25), $"Last Action: <b>{_lastAction}</b>");
            GUI.Label(new Rect(20, 135, 300, 25), $"Action Time: {_lastActionTime:F1}s (Now: {Time.time:F1}s)");

            GUI.Label(new Rect(20, 165, 300, 45), "<size=10>Controls: WASD=Move | Shift=Sprint | Space=Jump\nLMB/RMB=Attack | E=Skill | F=Interact | Tab=Inv | Esc=Pause</size>");
        }
    }
}
