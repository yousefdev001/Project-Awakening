using Awakening.Interaction;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to spawn interactive objects (Treasure Chests, Campfires) in front of the player.
    /// </summary>
    public class InteractionDebugSpawner : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 120;
            int boxX = Screen.width - 220;
            int boxY = 480;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🏰 Interaction Spawner (Phase 18)");

            int btnY = boxY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, 26), "📦 Spawn Treasure Chest"))
            {
                SpawnChest();
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 30, boxW - 20, 26), "🔥 Spawn Campfire Rest"))
            {
                SpawnCampfire();
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 60, boxW - 20, 22), "Clear Objects"))
            {
                ClearSpawnedObjects();
            }
        }

        private void SpawnChest()
        {
            Transform player = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = player != null ? player.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 spawnPos = (player != null ? player.position : Vector3.zero) + forward * 3.0f;
            spawnPos.y = 0.4f;

            GameObject chestObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestObj.name = "Treasure_Chest";
            chestObj.transform.position = spawnPos;
            chestObj.transform.localScale = new Vector3(1.2f, 0.8f, 0.8f);

            Renderer rend = chestObj.GetComponent<Renderer>();
            if (rend != null) rend.material.color = new Color(0.55f, 0.35f, 0.15f);

            chestObj.AddComponent<TreasureChest>();
            Debug.Log($"<color=#00FFAA>[InteractionSpawner]</color> Spawned Treasure Chest at {spawnPos}!");
        }

        private void SpawnCampfire()
        {
            Transform player = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = player != null ? player.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 spawnPos = (player != null ? player.position : Vector3.zero) + forward * 3.0f;
            spawnPos.y = 0.3f;

            GameObject fireObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fireObj.name = "Campfire_Site";
            fireObj.transform.position = spawnPos;
            fireObj.transform.localScale = new Vector3(1.0f, 0.2f, 1.0f);

            Renderer rend = fireObj.GetComponent<Renderer>();
            if (rend != null) rend.material.color = new Color(1.0f, 0.45f, 0.1f);

            fireObj.AddComponent<CampfireRestPoint>();
            Debug.Log($"<color=#FF7700>[InteractionSpawner]</color> Spawned Campfire Site at {spawnPos}!");
        }

        private void ClearSpawnedObjects()
        {
            TreasureChest[] chests = Object.FindObjectsByType<TreasureChest>(FindObjectsSortMode.None);
            foreach (var c in chests) Destroy(c.gameObject);

            CampfireRestPoint[] fires = Object.FindObjectsByType<CampfireRestPoint>(FindObjectsSortMode.None);
            foreach (var f in fires) Destroy(f.gameObject);

            Debug.Log("<color=#FFAA00>[InteractionSpawner]</color> Cleared interactive objects.");
        }
    }
}
