using Awakening.Interaction;
using Awakening.Items;
using Awakening.NPCs;
using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Procedurally constructs and layouts the complete Oakhaven Village hub.
    /// Spawns the Monolith Plaza, Elder's Sanctuary, Blacksmith Forge, Alchemist Shop, Houses, and Forest Gate.
    /// </summary>
    public class VillageGenerator : MonoBehaviour
    {
        public static VillageGenerator Instance { get; private set; }

        [Header("Auto-Build Settings")]
        [SerializeField] private bool _buildOnStart = true;

        private GameObject _villageRoot;

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
                BuildVillage();
            }
        }

        public void BuildVillage()
        {
            ClearVillage();

            _villageRoot = new GameObject("[Oakhaven_Village_Environment]");
            _villageRoot.transform.position = Vector3.zero;
            _villageRoot.AddComponent<VillageZoneTrigger>();

            // 1. Central Plaza & Awakening Monolith
            BuildPlaza(_villageRoot.transform);

            // 2. Elder's Sanctuary & NPC Eldrin
            BuildElderSanctuary(_villageRoot.transform);

            // 3. Blacksmith Forge & NPC Garrick
            BuildBlacksmithForge(_villageRoot.transform);

            // 4. Alchemist Shop & NPC Lyra
            BuildAlchemistShop(_villageRoot.transform);

            // 5. Residential Cabins
            BuildResidentialCabins(_villageRoot.transform);

            // 6. North Forest Gate (Exit to Forest)
            BuildNorthForestGate(_villageRoot.transform);

            // 7. Lantern Posts along paths
            BuildStreetLanterns(_villageRoot.transform);

            Debug.Log("<color=#00FFAA>🏰 [VillageGenerator]</color> Oakhaven Village successfully built!");
        }

        public void ClearVillage()
        {
            if (_villageRoot != null)
            {
                Destroy(_villageRoot);
                _villageRoot = null;
            }

            GameObject oldRoot = GameObject.Find("[Oakhaven_Village_Environment]");
            if (oldRoot != null)
            {
                Destroy(oldRoot);
            }
        }

        #region Landmark Builders

        private void BuildPlaza(Transform parent)
        {
            GameObject plazaGroup = new GameObject("01_Central_Plaza");
            plazaGroup.transform.SetParent(parent);

            // Plaza Cobblestone Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floor.name = "Plaza_Cobblestone_Floor";
            floor.transform.SetParent(plazaGroup.transform);
            floor.transform.position = new Vector3(0, 0.02f, 0);
            floor.transform.localScale = new Vector3(14f, 0.05f, 14f);
            SetMaterialColor(floor, new Color(0.45f, 0.45f, 0.48f));

            // Awakening Monolith (Ancient Obelisk)
            GameObject monolith = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monolith.name = "Awakening_Monolith_Obelisk";
            monolith.transform.SetParent(plazaGroup.transform);
            monolith.transform.position = new Vector3(0, 2.5f, 0);
            monolith.transform.localScale = new Vector3(1.2f, 5.0f, 1.2f);
            SetMaterialColor(monolith, new Color(0.2f, 0.5f, 0.85f)); // Mystic Blue

            // Monolith Glowing Top Rune
            GameObject rune = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rune.name = "Monolith_Rune_Crystal";
            rune.transform.SetParent(monolith.transform);
            rune.transform.localPosition = new Vector3(0, 0.6f, 0);
            rune.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            SetMaterialColor(rune, new Color(1f, 0.85f, 0.2f)); // Glowing Gold

            // Campfire Rest Site in Plaza
            GameObject campfire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            campfire.name = "Plaza_Campfire_Rest";
            campfire.transform.SetParent(plazaGroup.transform);
            campfire.transform.position = new Vector3(0, 0.15f, 3.5f);
            campfire.transform.localScale = new Vector3(1.2f, 0.25f, 1.2f);
            SetMaterialColor(campfire, new Color(1.0f, 0.45f, 0.1f));
            campfire.AddComponent<CampfireRestPoint>();
        }

        private void BuildElderSanctuary(Transform parent)
        {
            GameObject elderGroup = new GameObject("02_Elder_Sanctuary");
            elderGroup.transform.SetParent(parent);
            Vector3 basePos = new Vector3(-12f, 0, 6f);

            // Elder's Lodge Building
            CreateCabin(elderGroup.transform, basePos, new Vector3(6f, 3.5f, 5f), new Color(0.35f, 0.25f, 0.18f), "Elder's Lodge");

            // Spawn Elder Eldrin NPC
            SpawnNPC(elderGroup.transform, basePos + new Vector3(0, 0, -3.5f), NPCData.CreateElderPreset(), new Color(0.35f, 0.75f, 1.0f));

            // Treasure Chest behind lodge
            CreateChest(elderGroup.transform, basePos + new Vector3(-3.5f, 0, 1.5f));
        }

        private void BuildBlacksmithForge(Transform parent)
        {
            GameObject forgeGroup = new GameObject("03_Blacksmith_Forge");
            forgeGroup.transform.SetParent(parent);
            Vector3 basePos = new Vector3(12f, 0, 6f);

            // Forge Shelter (Open Roofed Workshop)
            CreateCabin(forgeGroup.transform, basePos, new Vector3(6f, 3.2f, 5f), new Color(0.3f, 0.28f, 0.26f), "Blacksmith Workshop");

            // Anvil Block
            GameObject anvil = GameObject.CreatePrimitive(PrimitiveType.Cube);
            anvil.name = "Forge_Anvil";
            anvil.transform.SetParent(forgeGroup.transform);
            anvil.transform.position = basePos + new Vector3(-1.5f, 0.45f, -2.5f);
            anvil.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
            SetMaterialColor(anvil, new Color(0.2f, 0.2f, 0.2f));

            // Spawn Blacksmith Garrick NPC
            SpawnNPC(forgeGroup.transform, basePos + new Vector3(0.5f, 0, -2.8f), NPCData.CreateBlacksmithPreset(), new Color(0.95f, 0.45f, 0.15f));
        }

        private void BuildAlchemistShop(Transform parent)
        {
            GameObject shopGroup = new GameObject("04_Alchemist_Shop");
            shopGroup.transform.SetParent(parent);
            Vector3 basePos = new Vector3(9f, 0, -8f);

            // Apothecary Hut
            CreateCabin(shopGroup.transform, basePos, new Vector3(5f, 3.0f, 4.5f), new Color(0.25f, 0.35f, 0.25f), "Apothecary Hut");

            // Potion Table / Stall
            GameObject stall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stall.name = "Potion_Stall_Table";
            stall.transform.SetParent(shopGroup.transform);
            stall.transform.position = basePos + new Vector3(0, 0.45f, -2.8f);
            stall.transform.localScale = new Vector3(2.5f, 0.9f, 0.8f);
            SetMaterialColor(stall, new Color(0.4f, 0.3f, 0.2f));

            // Spawn Alchemist Lyra NPC
            SpawnNPC(shopGroup.transform, basePos + new Vector3(0, 0, -4.0f), NPCData.CreateMerchantPreset(), new Color(0.85f, 0.35f, 0.95f));
        }

        private void BuildResidentialCabins(Transform parent)
        {
            GameObject resGroup = new GameObject("05_Residential_Cabins");
            resGroup.transform.SetParent(parent);

            // South-West Cabin
            CreateCabin(resGroup.transform, new Vector3(-9f, 0, -8f), new Vector3(5f, 3.2f, 4.5f), new Color(0.42f, 0.32f, 0.22f), "Southwest Cottage");

            // West Cabin
            CreateCabin(resGroup.transform, new Vector3(-13f, 0, -1f), new Vector3(4.5f, 3.0f, 4.0f), new Color(0.38f, 0.28f, 0.20f), "West Cottage");
        }

        private void BuildNorthForestGate(Transform parent)
        {
            GameObject gateGroup = new GameObject("06_North_Forest_Gate");
            gateGroup.transform.SetParent(parent);
            Vector3 gatePos = new Vector3(0, 0, 18f);

            // Left Gate Pillar
            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPost.name = "Gate_Left_Pillar";
            leftPost.transform.SetParent(gateGroup.transform);
            leftPost.transform.position = gatePos + new Vector3(-3.5f, 2.5f, 0);
            leftPost.transform.localScale = new Vector3(1.2f, 5.0f, 1.2f);
            SetMaterialColor(leftPost, new Color(0.3f, 0.2f, 0.15f));

            // Right Gate Pillar
            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPost.name = "Gate_Right_Pillar";
            rightPost.transform.SetParent(gateGroup.transform);
            rightPost.transform.position = gatePos + new Vector3(3.5f, 2.5f, 0);
            rightPost.transform.localScale = new Vector3(1.2f, 5.0f, 1.2f);
            SetMaterialColor(rightPost, new Color(0.3f, 0.2f, 0.15f));

            // Gate Top Arch
            GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arch.name = "Gate_Top_Arch";
            arch.transform.SetParent(gateGroup.transform);
            arch.transform.position = gatePos + new Vector3(0, 4.8f, 0);
            arch.transform.localScale = new Vector3(8.0f, 0.8f, 1.0f);
            SetMaterialColor(arch, new Color(0.35f, 0.22f, 0.15f));

            // Gate Sign / Lantern
            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sign.name = "Gate_Rune_Lantern";
            sign.transform.SetParent(arch.transform);
            sign.transform.localPosition = new Vector3(0, -0.6f, 0);
            sign.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            SetMaterialColor(sign, new Color(1f, 0.85f, 0.2f));
        }

        private void BuildStreetLanterns(Transform parent)
        {
            GameObject lanternGroup = new GameObject("07_Street_Lanterns");
            lanternGroup.transform.SetParent(parent);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-4.5f, 0, 5.5f),
                new Vector3(4.5f, 0, 5.5f),
                new Vector3(-4.0f, 0, -4.5f),
                new Vector3(4.0f, 0, -4.5f),
                new Vector3(0, 0, 11.0f)
            };

            foreach (var pos in positions)
            {
                CreateLantern(lanternGroup.transform, pos);
            }
        }

        #endregion

        #region Helper Constructors

        private void CreateCabin(Transform parent, Vector3 position, Vector3 size, Color woodColor, string name)
        {
            GameObject cabin = new GameObject(name);
            cabin.transform.SetParent(parent);
            cabin.transform.position = position;

            // Main Walls
            GameObject walls = GameObject.CreatePrimitive(PrimitiveType.Cube);
            walls.name = "Walls";
            walls.transform.SetParent(cabin.transform);
            walls.transform.localPosition = new Vector3(0, size.y / 2, 0);
            walls.transform.localScale = size;
            SetMaterialColor(walls, woodColor);

            // Roof (Slanted Peak)
            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Roof";
            roof.transform.SetParent(cabin.transform);
            roof.transform.localPosition = new Vector3(0, size.y + 0.4f, 0);
            roof.transform.localScale = new Vector3(size.x + 0.6f, 0.8f, size.z + 0.6f);
            roof.transform.localRotation = Quaternion.Euler(0, 0, 10f);
            SetMaterialColor(roof, new Color(0.45f, 0.18f, 0.15f)); // Terracotta Roof
        }

        private void SpawnNPC(Transform parent, Vector3 position, NPCData data, Color color)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = $"NPC_{data.npcName.Replace(" ", "_")}";
            npcObj.transform.SetParent(parent);
            npcObj.transform.position = position;
            npcObj.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

            SetMaterialColor(npcObj, color);

            NPCController controller = npcObj.AddComponent<NPCController>();
            controller.SetNPCData(data);
        }

        private void CreateChest(Transform parent, Vector3 position)
        {
            GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chest.name = "Treasure_Chest";
            chest.transform.SetParent(parent);
            chest.transform.position = position;
            chest.transform.localScale = new Vector3(1.0f, 0.7f, 0.7f);
            SetMaterialColor(chest, new Color(0.55f, 0.35f, 0.15f));
            chest.AddComponent<TreasureChest>();
        }

        private void CreateLantern(Transform parent, Vector3 position)
        {
            GameObject lantern = new GameObject("Lantern_Post");
            lantern.transform.SetParent(parent);
            lantern.transform.position = position;

            // Wooden Post
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            post.name = "Post";
            post.transform.SetParent(lantern.transform);
            post.transform.localPosition = new Vector3(0, 1.25f, 0);
            post.transform.localScale = new Vector3(0.15f, 1.25f, 0.15f);
            SetMaterialColor(post, new Color(0.3f, 0.22f, 0.15f));

            // Glowing Lantern Bulb
            GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.name = "Bulb";
            bulb.transform.SetParent(lantern.transform);
            bulb.transform.localPosition = new Vector3(0, 2.5f, 0);
            bulb.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            SetMaterialColor(bulb, new Color(1f, 0.88f, 0.25f));
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
