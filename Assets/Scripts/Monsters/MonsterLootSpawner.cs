using System.Collections.Generic;
using Awakening.Combat;
using Awakening.Items;
using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// Spawns physical world loot pickups when the monster dies, based on its assigned LootTable.
    /// </summary>
    [RequireComponent(typeof(MonsterStats))]
    [RequireComponent(typeof(HealthSystem))]
    public class MonsterLootSpawner : MonoBehaviour
    {
        [Header("Loot Configuration")]
        [SerializeField] private LootTable _lootTable;

        private MonsterStats _stats;
        private HealthSystem _healthSystem;

        private void Awake()
        {
            _stats = GetComponent<MonsterStats>();
            _healthSystem = GetComponent<HealthSystem>();
        }

        private void Start()
        {
            if (_lootTable == null && _stats != null && _stats.Data != null)
            {
                // Auto-assign appropriate MVP preset table
                string id = _stats.Data.monsterID;
                if (id.Contains("WOLF"))
                {
                    _lootTable = LootTable.CreateWolfLootTable();
                }
                else if (id.Contains("GOBLIN"))
                {
                    _lootTable = LootTable.CreateGoblinLootTable();
                }
                else
                {
                    _lootTable = LootTable.CreateSlimeLootTable();
                }
            }

            if (_healthSystem != null)
            {
                _healthSystem.OnDeath += HandleDeathLootDrop;
            }
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDeath -= HandleDeathLootDrop;
            }
        }

        private void HandleDeathLootDrop()
        {
            if (_lootTable == null) return;

            List<DroppedItemResult> drops = _lootTable.RollDrops();

            Vector3 spawnOrigin = transform.position;
            spawnOrigin.y = Mathf.Max(0.5f, spawnOrigin.y);

            foreach (var drop in drops)
            {
                // Scatter drops slightly in a circle
                Vector2 randCircle = Random.insideUnitCircle * 1.5f;
                Vector3 dropPos = spawnOrigin + new Vector3(randCircle.x, 0.4f, randCircle.y);

                SpawnWorldPickup(drop.Item, drop.Quantity, dropPos);
            }
        }

        private void SpawnWorldPickup(ItemData item, int quantity, Vector3 position)
        {
            GameObject pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObj.name = $"Drop_{item.itemName.Replace(" ", "_")}";
            pickupObj.transform.position = position;
            pickupObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            // Add WorldItemPickup
            WorldItemPickup pickup = pickupObj.AddComponent<WorldItemPickup>();
            pickup.Setup(item, quantity);
        }
    }
}
