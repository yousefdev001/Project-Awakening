using Awakening.NPCs;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to spawn Village NPCs (Elder, Blacksmith, Merchant) in front of the player.
    /// </summary>
    public class NPCSpawnerDebug : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 150;
            int boxX = Screen.width - 220;
            int boxY = 610;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🧙‍♂️ Village NPCs (Phase 19)");

            int btnY = boxY + 28;
            int btnH = 26;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "🧙‍♂️ Spawn Village Elder"))
            {
                SpawnNPC(NPCData.CreateElderPreset(), new Color(0.35f, 0.75f, 1.0f));
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 30, boxW - 20, btnH), "🔨 Spawn Blacksmith"))
            {
                SpawnNPC(NPCData.CreateBlacksmithPreset(), new Color(0.95f, 0.45f, 0.15f));
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 60, boxW - 20, btnH), "🧪 Spawn Alchemist"))
            {
                SpawnNPC(NPCData.CreateMerchantPreset(), new Color(0.85f, 0.35f, 0.95f));
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 92, boxW - 20, 22), "Clear All NPCs"))
            {
                ClearNPCs();
            }
        }

        private void SpawnNPC(NPCData data, Color color)
        {
            Transform player = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = player != null ? player.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 spawnPos = (player != null ? player.position : Vector3.zero) + forward * 2.8f;
            spawnPos.y = 0.5f;

            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = $"NPC_{data.npcName.Replace(" ", "_")}";
            npcObj.transform.position = spawnPos;
            npcObj.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

            // Face opposite to player initially
            npcObj.transform.rotation = Quaternion.LookRotation(-forward);

            Renderer rend = npcObj.GetComponent<Renderer>();
            if (rend != null) rend.material.color = color;

            NPCController controller = npcObj.AddComponent<NPCController>();
            controller.SetNPCData(data);

            Debug.Log($"<color=#00FFAA>[NPCSpawner]</color> Spawned <b>{data.npcName}</b> ({data.npcRole}) at {spawnPos}!");
        }

        private void ClearNPCs()
        {
            NPCController[] npcs = Object.FindObjectsByType<NPCController>(FindObjectsSortMode.None);
            foreach (var n in npcs) Destroy(n.gameObject);
            Debug.Log("<color=#FFAA00>[NPCSpawner]</color> Cleared NPCs from scene.");
        }
    }
}
