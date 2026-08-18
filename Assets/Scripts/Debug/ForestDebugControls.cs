using Awakening.World;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to rebuild the Forest layout or teleport across Forest combat zones.
    /// </summary>
    public class ForestDebugControls : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 150;
            int boxX = Screen.width - 220;
            int boxY = 760;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🌲 Forest Explorer (Phase 22)");

            int btnY = boxY + 26;
            int btnH = 25;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "🌲 Rebuild Forest Zone"))
            {
                if (ForestGenerator.Instance != null)
                {
                    ForestGenerator.Instance.BuildForest();
                }
            }

            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 95, 24), "🐺 Wolf Den"))
            {
                TeleportPlayer(new Vector3(-12.0f, 0.5f, 36.0f));
            }

            if (GUI.Button(new Rect(boxX + 110, btnY2, 90, 24), "👺 Goblins"))
            {
                TeleportPlayer(new Vector3(10.0f, 0.5f, 62.0f));
            }

            int btnY3 = btnY2 + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY3, 95, 24), "🔥 Clearing"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 48.0f));
            }

            if (GUI.Button(new Rect(boxX + 110, btnY3, 90, 24), "⛩️ Nest Gate"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 80.0f));
            }
        }

        private void TeleportPlayer(Vector3 targetPos)
        {
            Transform player = Camera.main != null ? Camera.main.transform.root : null;
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.position = targetPos;
                if (cc != null) cc.enabled = true;
                Debug.Log($"<color=#00FFAA>[Teleport]</color> Teleported player to {targetPos}.");
            }
        }
    }
}
