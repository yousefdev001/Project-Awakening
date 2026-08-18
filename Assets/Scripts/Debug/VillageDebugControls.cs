using Awakening.World;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to rebuild the Village layout, clear objects, or teleport across village landmarks.
    /// </summary>
    public class VillageDebugControls : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 145;
            int boxX = Screen.width - 220;
            int boxY = 610;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🏘️ Village Builder (Phase 21)");

            int btnY = boxY + 26;
            int btnH = 25;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "🏰 Rebuild Oakhaven Village"))
            {
                if (VillageGenerator.Instance != null)
                {
                    VillageGenerator.Instance.BuildVillage();
                }
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 28, boxW - 20, btnH), "🧹 Clear Village"))
            {
                if (VillageGenerator.Instance != null)
                {
                    VillageGenerator.Instance.ClearVillage();
                }
            }

            int btnY2 = btnY + 58;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 95, 24), "📍 Monolith"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 2.0f));
            }

            if (GUI.Button(new Rect(boxX + 110, btnY2, 90, 24), "⛩️ Gate"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 15.0f));
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
