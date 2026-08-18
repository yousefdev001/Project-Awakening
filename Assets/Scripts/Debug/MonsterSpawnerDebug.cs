using Awakening.Combat;
using Awakening.Monsters;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to spawn different monster types in front of the player for combat and XP testing.
    /// </summary>
    public class MonsterSpawnerDebug : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 175;
            int boxX = Screen.width - 220;
            int boxY = 300;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "👾 Monster Spawner (Phase 13)");

            int btnY = boxY + 28;
            int btnH = 26;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "🟢 Spawn Slime (Lv. 1)"))
            {
                SpawnMonster(MonsterData.CreateSlimePreset(), PrimitiveType.Sphere);
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 30, boxW - 20, btnH), "🐺 Spawn Wolf (Lv. 3)"))
            {
                SpawnMonster(MonsterData.CreateWolfPreset(), PrimitiveType.Capsule);
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 60, boxW - 20, btnH), "👺 Spawn Goblin (Lv. 5)"))
            {
                SpawnMonster(MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 95, boxW - 20, 22), "Clear All Monsters"))
            {
                ClearMonsters();
            }
        }

        private void SpawnMonster(MonsterData data, PrimitiveType primitive)
        {
            Transform player = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = player != null ? player.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 spawnPos = (player != null ? player.position : Vector3.zero) + forward * 4.0f;
            spawnPos.y = 0.5f;

            GameObject monsterObj = GameObject.CreatePrimitive(primitive);
            monsterObj.name = $"Monster_{data.monsterName.Replace(" ", "_")}";
            monsterObj.transform.position = spawnPos;

            // Set Tag and Layer
            monsterObj.tag = "Untagged";

            // Add Components
            HealthSystem hp = monsterObj.AddComponent<HealthSystem>();
            MonsterStats stats = monsterObj.AddComponent<MonsterStats>();
            MonsterCombat combat = monsterObj.AddComponent<MonsterCombat>();
            MonsterController ai = monsterObj.AddComponent<MonsterController>();

            // Assign Data
            stats.SetMonsterData(data);

            Debug.Log($"<color=#00FFAA>[MonsterSpawner]</color> Spawned <b>[{data.rank}] {data.monsterName}</b> at {spawnPos}!");
        }

        private void ClearMonsters()
        {
            MonsterStats[] monsters = Object.FindObjectsByType<MonsterStats>(FindObjectsSortMode.None);
            foreach (var m in monsters)
            {
                Destroy(m.gameObject);
            }
            Debug.Log($"<color=#FFAA00>[MonsterSpawner]</color> Cleared {monsters.Length} monsters from scene.");
        }
    }
}
