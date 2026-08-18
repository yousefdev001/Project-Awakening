using Awakening.Persistence;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Interactive Save & Load Persistence debug panel.
    /// Allows saving, loading, inspecting save metadata, or wiping local JSON saves.
    /// </summary>
    public class SaveDebugPanel : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            SaveSystem ss = SaveSystem.Instance;
            if (ss == null) return;

            int boxW = 210;
            int boxH = 150;
            int boxX = 10;
            int boxY = 910;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "💾 Save & Persistence (Phase 27)");

            string status = SaveSystem.HasSaveFile ? "<color=#00FFAA>Save File: READY</color>" : "<color=grey>Save File: NONE</color>";
            GUI.Label(new Rect(boxX + 10, boxY + 22, boxW - 20, 20), $"<size=10><b>{status}</b></size>");

            int btnY = boxY + 44;
            int btnH = 26;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "💾 Save Current Game"))
            {
                ss.SaveGame();
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 30, boxW - 20, btnH), "📂 Load Saved Game"))
            {
                ss.LoadGame();
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 62, boxW - 20, 22), "🗑️ Delete Save File"))
            {
                ss.DeleteSaveFile();
            }
        }
    }
}
