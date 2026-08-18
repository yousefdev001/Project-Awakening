using Awakening.Combat;
using Awakening.Interaction;
using Awakening.Items;
using Awakening.Monsters;
using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Procedurally constructs and layouts the Goblin Nest Dungeon (Z: 120 to Z: 210).
    /// Spawns Entry Chamber, Trapped Chasm, Goblin Treasury, Boss Ante-Chamber, and Portals.
    /// </summary>
    public class NestGenerator : MonoBehaviour
    {
        public static NestGenerator Instance { get; private set; }

        [Header("Auto-Build Settings")]
        [SerializeField] private bool _buildOnStart = true;

        private GameObject _nestRoot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (_buildOnStart)
            {
                BuildNest();
            }
        }

        public void BuildNest()
        {
            ClearNest();

            _nestRoot = new GameObject("[Goblin_Nest_Dungeon_Environment]");
            _nestRoot.transform.position = Vector3.zero;
            _nestRoot.AddComponent<NestZoneTrigger>();

            // 1. Entrance Chamber & Return Portal (Z: 130)
            BuildEntranceChamber(_nestRoot.transform);

            // 2. Trapped Chasm Corridor (Z: 152)
            BuildChasmCorridor(_nestRoot.transform);

            // 3. Goblin Treasury & Elite Barracks (Z: 175)
            BuildTreasuryChamber(_nestRoot.transform);

            // 4. Boss Arena Ante-Chamber & Great Gate (Z: 200)
            BuildBossAnteChamber(_nestRoot.transform);

            // 5. Connect Forest Portal to Nest Entrance
            LinkForestPortal();

            Debug.Log("<color=#00FFAA>👹 [NestGenerator]</color> Goblin Nest Dungeon successfully generated!");
        }

        public void ClearNest()
        {
            if (_nestRoot != null)
            {
                Destroy(_nestRoot);
                _nestRoot = null;
            }

            GameObject oldRoot = GameObject.Find("[Goblin_Nest_Dungeon_Environment]");
            if (oldRoot != null)
            {
                Destroy(oldRoot);
            }
        }

        #region Chamber Builders

        private void BuildEntranceChamber(Transform parent)
        {
            GameObject room = new GameObject("01_Entrance_Chamber");
            room.transform.SetParent(parent);
            Vector3 center = new Vector3(0, 0, 130f);

            // Dungeon Stone Floor
            CreateFloor(room.transform, center, new Vector3(16f, 0.05f, 16f), new Color(0.18f, 0.18f, 0.22f));

            // Cavern Boundary Walls
            CreateWall(room.transform, center + new Vector3(-8f, 2.5f, 0), new Vector3(0.8f, 5f, 16f));
            CreateWall(room.transform, center + new Vector3(8f, 2.5f, 0), new Vector3(0.8f, 5f, 16f));
            CreateWall(room.transform, center + new Vector3(-4.5f, 2.5f, -8f), new Vector3(7f, 5f, 0.8f));
            CreateWall(room.transform, center + new Vector3(4.5f, 2.5f, -8f), new Vector3(7f, 5f, 0.8f));

            // Return Portal back to Forest
            GameObject returnPortal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            returnPortal.name = "Return_Portal_To_Forest";
            returnPortal.transform.SetParent(room.transform);
            returnPortal.transform.position = center + new Vector3(0, 0.1f, -6.5f);
            returnPortal.transform.localScale = new Vector3(1.6f, 0.2f, 1.6f);
            DungeonPortal portal = returnPortal.AddComponent<DungeonPortal>();
            portal.Setup("Return to Whispering Forest", new Vector3(0, 0.5f, 82f), new Color(0.2f, 0.8f, 1.0f));

            // Fire Braziers
            CreateBrazier(room.transform, center + new Vector3(-6f, 0, 6f));
            CreateBrazier(room.transform, center + new Vector3(6f, 0, 6f));

            // 3 Entrance Guard Goblins
            SpawnMonster(room.transform, center + new Vector3(-3f, 0, 2f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(room.transform, center + new Vector3(3f, 0, 2f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(room.transform, center + new Vector3(0, 0, 4f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
        }

        private void BuildChasmCorridor(Transform parent)
        {
            GameObject corridor = new GameObject("02_Chasm_Corridor");
            corridor.transform.SetParent(parent);
            Vector3 center = new Vector3(0, 0, 152f);

            // Narrow Stone Bridge Floor
            CreateFloor(corridor.transform, center, new Vector3(6f, 0.05f, 28f), new Color(0.22f, 0.22f, 0.25f));

            // Side Pillars & Chasms
            CreateWall(corridor.transform, center + new Vector3(-3.2f, 2.5f, 0), new Vector3(0.5f, 5f, 28f));
            CreateWall(corridor.transform, center + new Vector3(3.2f, 2.5f, 0), new Vector3(0.5f, 5f, 28f));

            // Supply Chest in niche
            CreateChest(corridor.transform, center + new Vector3(-1.8f, 0, 0));
        }

        private void BuildTreasuryChamber(Transform parent)
        {
            GameObject treasury = new GameObject("03_Goblin_Treasury");
            treasury.transform.SetParent(parent);
            Vector3 center = new Vector3(0, 0, 178f);

            // Wide Vault Floor
            CreateFloor(treasury.transform, center, new Vector3(20f, 0.05f, 20f), new Color(0.25f, 0.22f, 0.2f));

            // Vault Walls
            CreateWall(treasury.transform, center + new Vector3(-10f, 2.5f, 0), new Vector3(0.8f, 5f, 20f));
            CreateWall(treasury.transform, center + new Vector3(10f, 2.5f, 0), new Vector3(0.8f, 5f, 20f));

            // 2 Grand Treasure Chests
            CreateChest(treasury.transform, center + new Vector3(-5f, 0, 6f));
            CreateChest(treasury.transform, center + new Vector3(5f, 0, 6f));

            // 3 Vault Elite Goblins
            SpawnMonster(treasury.transform, center + new Vector3(-3.5f, 0, 0), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(treasury.transform, center + new Vector3(3.5f, 0, 0), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(treasury.transform, center + new Vector3(0, 0, 3f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
        }

        private void BuildBossAnteChamber(Transform parent)
        {
            GameObject bossAnte = new GameObject("04_Boss_AnteChamber");
            bossAnte.transform.SetParent(parent);
            Vector3 center = new Vector3(0, 0, 202f);

            // Ante-Chamber Floor
            CreateFloor(bossAnte.transform, center, new Vector3(16f, 0.05f, 16f), new Color(0.28f, 0.16f, 0.18f));

            // Campfire Rest Site before Boss
            GameObject campfire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            campfire.name = "Pre_Boss_Campfire";
            campfire.transform.SetParent(bossAnte.transform);
            campfire.transform.position = center + new Vector3(-4f, 0.15f, -2f);
            campfire.transform.localScale = new Vector3(1.2f, 0.25f, 1.2f);
            SetMaterialColor(campfire, new Color(1.0f, 0.45f, 0.1f));
            campfire.AddComponent<CampfireRestPoint>();

            // Great Boss Skull Gateway (Leading to Boss Arena in Phase 24)
            Vector3 gatePos = center + new Vector3(0, 0, 8f);

            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPost.name = "Boss_Gate_Left";
            leftPost.transform.SetParent(bossAnte.transform);
            leftPost.transform.position = gatePos + new Vector3(-3.5f, 3.5f, 0);
            leftPost.transform.localScale = new Vector3(1.5f, 7f, 1.5f);
            SetMaterialColor(leftPost, new Color(0.12f, 0.12f, 0.14f));

            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPost.name = "Boss_Gate_Right";
            rightPost.transform.SetParent(bossAnte.transform);
            rightPost.transform.position = gatePos + new Vector3(3.5f, 3.5f, 0);
            rightPost.transform.localScale = new Vector3(1.5f, 7f, 1.5f);
            SetMaterialColor(rightPost, new Color(0.12f, 0.12f, 0.14f));

            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Boss_Gate_Arch";
            arch.transform.SetParent(bossAnte.transform);
            arch.transform.position = gatePos + new Vector3(0, 6.8f, 0);
            arch.transform.localScale = new Vector3(8.5f, 1.2f, 1.5f);
            SetMaterialColor(arch, new Color(0.15f, 0.15f, 0.16f));

            GameObject rune = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rune.name = "Boss_Rune_Eye";
            rune.transform.SetParent(arch.transform);
            rune.transform.localPosition = new Vector3(0, -0.6f, 0);
            rune.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            SetMaterialColor(rune, new Color(1f, 0.1f, 0.1f)); // Blood Red Rune
        }

        private void LinkForestPortal()
        {
            // Find or create the Forest Portal trigger at Z: 85
            GameObject forestGate = GameObject.Find("Dungeon_Rune_Eye");
            if (forestGate != null)
            {
                DungeonPortal portal = forestGate.GetComponent<DungeonPortal>() ?? forestGate.AddComponent<DungeonPortal>();
                portal.Setup("Enter Goblin Nest Dungeon", new Vector3(0, 0.5f, 126f), new Color(1f, 0.15f, 0.15f));
            }
        }

        #endregion

        #region Helper Constructors

        private void CreateFloor(Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Cavern_Floor";
            floor.transform.SetParent(parent);
            floor.transform.position = position;
            floor.transform.localScale = scale;
            SetMaterialColor(floor, color);
        }

        private void CreateWall(Transform parent, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Cavern_Wall";
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            SetMaterialColor(wall, new Color(0.14f, 0.14f, 0.16f)); // Dark Cave Rock
        }

        private void CreateBrazier(Transform parent, Vector3 position)
        {
            GameObject brazier = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            brazier.name = "Cave_Brazier";
            brazier.transform.SetParent(parent);
            brazier.transform.position = position + new Vector3(0, 0.4f, 0);
            brazier.transform.localScale = new Vector3(0.7f, 0.8f, 0.7f);
            SetMaterialColor(brazier, new Color(0.9f, 0.2f, 0.1f));
        }

        private void CreateChest(Transform parent, Vector3 position)
        {
            GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chest.name = "Nest_Treasure_Chest";
            chest.transform.SetParent(parent);
            chest.transform.position = position + new Vector3(0, 0.35f, 0);
            chest.transform.localScale = new Vector3(1.1f, 0.75f, 0.75f);
            SetMaterialColor(chest, new Color(0.6f, 0.45f, 0.2f));
            chest.AddComponent<TreasureChest>();
        }

        private void SpawnMonster(Transform parent, Vector3 position, MonsterData data, PrimitiveType primitive)
        {
            GameObject monsterObj = GameObject.CreatePrimitive(primitive);
            monsterObj.name = $"Nest_{data.monsterName.Replace(" ", "_")}";
            monsterObj.transform.SetParent(parent);
            monsterObj.transform.position = position + new Vector3(0, 0.5f, 0);

            monsterObj.AddComponent<HealthSystem>();
            MonsterStats stats = monsterObj.AddComponent<MonsterStats>();
            monsterObj.AddComponent<MonsterCombat>();
            monsterObj.AddComponent<MonsterController>();
            monsterObj.AddComponent<MonsterLootSpawner>();

            stats.SetMonsterData(data);
        }

        private void SetMaterialColor(GameObject obj, Color color)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.color = color;
            }
        }

        #endregion
    }
}
