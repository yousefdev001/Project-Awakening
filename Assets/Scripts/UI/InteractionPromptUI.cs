using Awakening.Interaction;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// UI component rendering the on-screen [F] Interaction Prompt banner when near an interactable object.
    /// </summary>
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerInteraction interaction = PlayerInteraction.Instance;
            if (interaction == null || !interaction.HasInteractable) return;

            string promptText = interaction.CurrentInteractable.InteractionPrompt;
            if (string.IsNullOrEmpty(promptText)) return;

            int screenW = Screen.width;
            int screenH = Screen.height;

            int boxW = 280;
            int boxH = 42;
            int boxX = (screenW - boxW) / 2;
            int boxY = screenH - 170;

            // Background banner
            GUI.color = new Color(0.05f, 0.08f, 0.12f, 0.85f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // Gold accent border
            GUI.color = new Color(1f, 0.85f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(boxX, boxY + boxH - 2, boxW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Key Icon Box [F]
            int keyW = 28;
            int keyH = 28;
            int keyX = boxX + 12;
            int keyY = boxY + 7;

            GUI.color = new Color(1f, 0.9f, 0.3f);
            GUI.Box(new Rect(keyX, keyY, keyW, keyH), "");
            GUI.Label(new Rect(keyX, keyY + 4, keyW, keyH), "<b>F</b>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
            GUI.color = Color.white;

            // Prompt Action Text
            GUI.Label(new Rect(boxX + 48, boxY + 10, boxW - 55, 24), $"<size=12><b>{promptText}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft });
        }
    }
}
