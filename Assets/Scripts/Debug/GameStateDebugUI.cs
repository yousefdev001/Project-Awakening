using Awakening.Core;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to visualize and switch between GameStates in Play Mode.
    /// </summary>
    public class GameStateDebugUI : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            GameStateManager manager = GameStateManager.Instance;
            if (manager == null) return;

            GUI.Box(new Rect(Screen.width - 220, 10, 210, 240), "⚡ Game State Control (Phase 5)");

            GUI.Label(new Rect(Screen.width - 210, 35, 190, 25), $"Current: <color=yellow><b>{manager.CurrentState}</b></color>");
            GUI.Label(new Rect(Screen.width - 210, 55, 190, 25), $"TimeScale: <b>{Time.timeScale:F1}x</b>");

            int yOffset = 85;
            int buttonHeight = 22;
            int spacing = 25;

            if (GUI.Button(new Rect(Screen.width - 210, yOffset, 190, buttonHeight), "Gameplay (Resume)"))
            {
                manager.SetState(GameState.Gameplay);
            }

            if (GUI.Button(new Rect(Screen.width - 210, yOffset + spacing, 190, buttonHeight), "Pause (Esc)"))
            {
                manager.SetState(GameState.Paused);
            }

            if (GUI.Button(new Rect(Screen.width - 210, yOffset + spacing * 2, 190, buttonHeight), "Awakening Rite"))
            {
                manager.SetState(GameState.Awakening);
            }

            if (GUI.Button(new Rect(Screen.width - 210, yOffset + spacing * 3, 190, buttonHeight), "Character Creation"))
            {
                manager.SetState(GameState.CharacterCreation);
            }

            if (GUI.Button(new Rect(Screen.width - 210, yOffset + spacing * 4, 190, buttonHeight), "Game Over"))
            {
                manager.SetState(GameState.GameOver);
            }
        }
    }
}
