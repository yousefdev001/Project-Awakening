using Awakening.Professions;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug trigger button on screen to launch the full cinematic Awakening Ceremony.
    /// </summary>
    public class AwakeningDebugTrigger : MonoBehaviour
    {
        [SerializeField] private bool _showButton = true;

        private void OnGUI()
        {
            if (!_showButton) return;

            AwakeningController controller = AwakeningController.Instance;
            if (controller != null && controller.IsAwakening) return; // Hide button while ceremony is playing

            int btnW = 210;
            int btnH = 34;
            int btnX = Screen.width - 220;
            int btnY = 260;

            GUI.color = new Color(1f, 0.85f, 0.2f);
            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "✨ Start Awakening Rite ✨"))
            {
                if (controller != null)
                {
                    controller.StartAwakeningSequence();
                }
                else
                {
                    Debug.LogWarning("[AwakeningDebugTrigger] AwakeningController not found in the scene.");
                }
            }
            GUI.color = Color.white;
        }
    }
}
