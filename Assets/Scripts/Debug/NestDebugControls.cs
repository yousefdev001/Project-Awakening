using Awakening.World;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to rebuild the Goblin Nest layout or teleport directly inside the dungeon chambers.
    /// </summary>
    public class NestDebugControls : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 150;
            int boxX = Screen.width - 220;
            int boxY = 915;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "👹 Goblin Nest Dungeon (Phase 23)");

            int btnY = boxY + 26;
            int btnH = 25;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "👹 Rebuild Goblin Nest"))
            {
                if (NestGenerator.Instance != null)
                {
                    NestGenerator.Instance.BuildNest();
                }
            }

            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 95, 24), "🚪 Entrance"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 126.0f));
            }

            if (GUI.Button(new Rect(boxX + 110, btnY2, 90, 24), "💎 Treasury"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 175.0f));
            }

            int btnY3 = btnY2 + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY3, 95, 24), "👑 Boss Gate"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 200.0f));
            }

            if (GUI.Button(new Rect(boxX + 110, btnY3, 90, 24), "🏰 Village"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 2.0f));
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
