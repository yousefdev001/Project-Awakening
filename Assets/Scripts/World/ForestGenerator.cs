using Awakening.Combat;
using Awakening.Interaction;
using Awakening.Items;
using Awakening.Monsters;
using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Procedurally constructs and layouts the Whispering Forest zone (Z: 20 to Z: 90).
    /// Spawns Forest Paths, Pine Trees, Boulders, Wolf Den territory, Goblin Outpost, and Nest Gateway.
    /// </summary>
    public class ForestGenerator : MonoBehaviour
    {
        public static ForestGenerator Instance { get; private set; }

        [Header("Auto-Build Settings")]
        [SerializeField] private bool _buildOnStart = true;

        private GameObject _forestRoot;

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
                BuildForest();
            }
        }

        public void BuildForest()
        {
            ClearForest();

            _forestRoot = new GameObject("[Whispering_Forest_Environment]");
            _forestRoot.transform.position = Vector3.zero;
            _forestRoot.AddComponent<ForestZoneTrigger>();

            // 1. Forest Pathways
            BuildForestPathways(_forestRoot.transform);

            // 2. Tree Borders & Dense Foliage
            BuildForestFoliage(_forestRoot.transform);

            // 3. Wolf Den Territory (West Z: 35)
            BuildWolfDen(_forestRoot.transform);

            // 4. Midway Campfire Clearing (Z: 48)
            BuildMidwayClearing(_forestRoot.transform);

            // 5. Goblin Outpost Camp (East Z: 62)
            BuildGoblinOutpost(_forestRoot.transform);

            // 6. Ancient Goblin Nest Portal Gateway (North Z: 85)
            BuildNestGateway(_forestRoot.transform);

            Debug.Log("<color=#00FFAA>🌲 [ForestGenerator]</color> Whispering Forest successfully generated!");
        }

        public void ClearForest()
        {
            if (_forestRoot != null)
            {
                Destroy(_forestRoot);
                _forestRoot = null;
            }

            GameObject oldRoot = GameObject.Find("[Whispering_Forest_Environment]");
            if (oldRoot != null)
            {
                Destroy(oldRoot);
            }
        }

        #region Landmark Builders

        private void BuildForestPathways(Transform parent)
        {
            GameObject pathGroup = new GameObject("01_Forest_Pathway");
            pathGroup.transform.SetParent(parent);

            // Main dirt road from Z: 20 to Z: 85
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Main_Forest_Road";
            road.transform.SetParent(pathGroup.transform);
            road.transform.position = new Vector3(0, 0.02f, 52.5f);
            road.transform.localScale = new Vector3(6.0f, 0.04f, 65.0f);
            SetMaterialColor(road, new Color(0.38f, 0.32f, 0.22f)); // Dirt Brown
        }

        private void BuildForestFoliage(Transform parent)
        {
            GameObject foliageGroup = new GameObject("02_Trees_And_Boulders");
            foliageGroup.transform.SetParent(parent);

            // Place Pine Trees along left and right sides of the road
            for (float z = 24f; z <= 82f; z += 6f)
            {
                // Left Tree line
                CreatePineTree(foliageGroup.transform, new Vector3(-6.5f - Random.Range(0f, 4f), 0, z + Random.Range(-1.5f, 1.5f)));
                CreatePineTree(foliageGroup.transform, new Vector3(-12.0f - Random.Range(0f, 5f), 0, z + Random.Range(-2f, 2f)));

                // Right Tree line
                CreatePineTree(foliageGroup.transform, new Vector3(6.5f + Random.Range(0f, 4f), 0, z + Random.Range(-1.5f, 1.5f)));
                CreatePineTree(foliageGroup.transform, new Vector3(12.0f + Random.Range(0f, 5f), 0, z + Random.Range(-2f, 2f)));
            }

            // Scatter rocks and boulders
            Vector3[] rockPositions = new Vector3[]
            {
                new Vector3(-4.5f, 0, 28f),
                new Vector3(5.0f, 0, 36f),
                new Vector3(-5.5f, 0, 52f),
                new Vector3(4.8f, 0, 72f)
            };

            foreach (var pos in rockPositions)
            {
                CreateBoulder(foliageGroup.transform, pos, Random.Range(1.0f, 2.0f));
            }
        }

        private void BuildWolfDen(Transform parent)
        {
            GameObject wolfGroup = new GameObject("03_Wolf_Den_Territory");
            wolfGroup.transform.SetParent(parent);
            Vector3 denCenter = new Vector3(-14.0f, 0, 36.0f);

            // Den Rocky Formation
            CreateBoulder(wolfGroup.transform, denCenter + new Vector3(-3f, 0, 2f), 3.0f);
            CreateBoulder(wolfGroup.transform, denCenter + new Vector3(3f, 0, -2f), 2.5f);
            CreateBoulder(wolfGroup.transform, denCenter + new Vector3(0, 0, 4f), 2.8f);

            // Hidden Chest in Wolf Den
            CreateChest(wolfGroup.transform, denCenter + new Vector3(-1.5f, 0, 2.5f));

            // Spawn 3 Wild Wolves
            SpawnMonster(wolfGroup.transform, denCenter + new Vector3(0, 0, 0), MonsterData.CreateWolfPreset(), PrimitiveType.Capsule);
            SpawnMonster(wolfGroup.transform, denCenter + new Vector3(3f, 0, 2f), MonsterData.CreateWolfPreset(), PrimitiveType.Capsule);
            SpawnMonster(wolfGroup.transform, denCenter + new Vector3(-2.5f, 0, -2f), MonsterData.CreateWolfPreset(), PrimitiveType.Capsule);
        }

        private void BuildMidwayClearing(Transform parent)
        {
            GameObject clearGroup = new GameObject("04_Midway_Campfire");
            clearGroup.transform.SetParent(parent);
            Vector3 clearPos = new Vector3(0, 0, 48.0f);

            // Campfire Rest Site
            GameObject campfire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            campfire.name = "Midway_Campfire";
            campfire.transform.SetParent(clearGroup.transform);
            campfire.transform.position = clearPos + new Vector3(4.5f, 0.15f, 0);
            campfire.transform.localScale = new Vector3(1.2f, 0.25f, 1.2f);
            SetMaterialColor(campfire, new Color(1.0f, 0.45f, 0.1f));
            campfire.AddComponent<CampfireRestPoint>();
        }

        private void BuildGoblinOutpost(Transform parent)
        {
            GameObject goblinGroup = new GameObject("05_Goblin_Outpost");
            goblinGroup.transform.SetParent(parent);
            Vector3 campCenter = new Vector3(14.0f, 0, 64.0f);

            // Outpost Wooden Palisade Barricade
            for (float angle = -60f; angle <= 60f; angle += 30f)
            {
                Quaternion rot = Quaternion.Euler(0, angle, 0);
                Vector3 postPos = campCenter + rot * Vector3.forward * 5.0f;
                CreatePalisade(goblinGroup.transform, postPos);
            }

            // Goblin Bonfire
            GameObject brazier = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            brazier.name = "Goblin_War_Brazier";
            brazier.transform.SetParent(goblinGroup.transform);
            brazier.transform.position = campCenter;
            brazier.transform.localScale = new Vector3(1.0f, 0.8f, 1.0f);
            SetMaterialColor(brazier, new Color(0.85f, 0.25f, 0.1f));

            // Hidden Chest in Camp
            CreateChest(goblinGroup.transform, campCenter + new Vector3(2.5f, 0, 2.0f));

            // Spawn 3 Goblin Warriors
            SpawnMonster(goblinGroup.transform, campCenter + new Vector3(-2f, 0, 0), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(goblinGroup.transform, campCenter + new Vector3(2f, 0, -1.5f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
            SpawnMonster(goblinGroup.transform, campCenter + new Vector3(0, 0, 2.5f), MonsterData.CreateGoblinPreset(), PrimitiveType.Cube);
        }

        private void BuildNestGateway(Transform parent)
        {
            GameObject gateGroup = new GameObject("06_Goblin_Nest_Gateway");
            gateGroup.transform.SetParent(parent);
            Vector3 gatePos = new Vector3(0, 0, 85.0f);

            // Left Monolith
            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPost.name = "Nest_Left_Obelisk";
            leftPost.transform.SetParent(gateGroup.transform);
            leftPost.transform.position = gatePos + new Vector3(-4.0f, 3.5f, 0);
            leftPost.transform.localScale = new Vector3(1.6f, 7.0f, 1.6f);
            SetMaterialColor(leftPost, new Color(0.18f, 0.18f, 0.2f)); // Dark Obsidian

            // Right Monolith
            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPost.name = "Nest_Right_Obelisk";
            rightPost.transform.SetParent(gateGroup.transform);
            rightPost.transform.position = gatePos + new Vector3(4.0f, 3.5f, 0);
            rightPost.transform.localScale = new Vector3(1.6f, 7.0f, 1.6f);
            SetMaterialColor(rightPost, new Color(0.18f, 0.18f, 0.2f));

            // Archway Top
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Nest_Arch_Top";
            arch.transform.SetParent(gateGroup.transform);
            arch.transform.position = gatePos + new Vector3(0, 6.8f, 0);
            arch.transform.localScale = new Vector3(9.5f, 1.2f, 1.4f);
            SetMaterialColor(arch, new Color(0.15f, 0.15f, 0.16f));

            // Glowing Crimson Dungeon Rune
            GameObject rune = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rune.name = "Dungeon_Rune_Eye";
            rune.transform.SetParent(arch.transform);
            rune.transform.localPosition = new Vector3(0, -0.6f, 0);
            rune.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            SetMaterialColor(rune, new Color(1f, 0.15f, 0.15f)); // Crimson Blood Eye
        }

        #endregion

        #region Helper Constructors

        private void CreatePineTree(Transform parent, Vector3 position)
        {
            GameObject tree = new GameObject("Pine_Tree");
            tree.transform.SetParent(parent);
            tree.transform.position = position;

            // Trunk
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
            SetMaterialColor(trunk, new Color(0.32f, 0.22f, 0.14f));

            // Foliage Cone (Capsule/Cylinder)
            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            foliage.name = "Foliage";
            foliage.transform.SetParent(tree.transform);
            foliage.transform.localPosition = new Vector3(0, 4.0f, 0);
            foliage.transform.localScale = new Vector3(1.8f, 2.5f, 1.8f);
            SetMaterialColor(foliage, new Color(0.12f, 0.38f, 0.18f)); // Deep Forest Green
        }

        private void CreateBoulder(Transform parent, Vector3 position, float scale)
        {
            GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boulder.name = "Forest_Boulder";
            boulder.transform.SetParent(parent);
            boulder.transform.position = position + new Vector3(0, scale * 0.4f, 0);
            boulder.transform.localScale = new Vector3(scale * 1.2f, scale * 0.8f, scale * 1.1f);
            SetMaterialColor(boulder, new Color(0.45f, 0.46f, 0.48f));
        }

        private void CreatePalisade(Transform parent, Vector3 position)
        {
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "Palisade_Post";
            post.transform.SetParent(parent);
            post.transform.position = position + new Vector3(0, 1.2f, 0);
            post.transform.localScale = new Vector3(0.3f, 1.2f, 0.3f);
            SetMaterialColor(post, new Color(0.35f, 0.25f, 0.18f));
        }

        private void CreateChest(Transform parent, Vector3 position)
        {
            GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chest.name = "Hidden_Forest_Chest";
            chest.transform.SetParent(parent);
            chest.transform.position = position + new Vector3(0, 0.35f, 0);
            chest.transform.localScale = new Vector3(1.0f, 0.7f, 0.7f);
            SetMaterialColor(chest, new Color(0.55f, 0.35f, 0.15f));
            chest.AddComponent<TreasureChest>();
        }

        private void SpawnMonster(Transform parent, Vector3 position, MonsterData data, PrimitiveType primitive)
        {
            GameObject monsterObj = GameObject.CreatePrimitive(primitive);
            monsterObj.name = $"Monster_{data.monsterName.Replace(" ", "_")}";
            monsterObj.transform.SetParent(parent);
            monsterObj.transform.position = position + new Vector3(0, 0.5f, 0);

            // Add Standard Monster Architecture Components
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
