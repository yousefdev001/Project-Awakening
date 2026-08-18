using Awakening.Interaction;
using Awakening.Monsters;
using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Procedurally constructs and layouts the Grand Colosseum Boss Arena for Gorgar the Goblin Chief.
    /// </summary>
    public class BossArenaGenerator : MonoBehaviour
    {
        public static BossArenaGenerator Instance { get; private set; }

        [Header("Auto-Build Settings")]
        [SerializeField] private bool _buildOnStart = true;

        private GameObject _arenaRoot;

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
                BuildBossArena();
            }
        }

        public void BuildBossArena()
        {
            ClearBossArena();

            _arenaRoot = new GameObject("[Goblin_Chief_Boss_Arena]");
            _arenaRoot.transform.position = Vector3.zero;

            Vector3 arenaCenter = new Vector3(0, 0, 235.0f);

            // 1. Colosseum Circular Arena Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floor.name = "Arena_Colosseum_Floor";
            floor.transform.SetParent(_arenaRoot.transform);
            floor.transform.position = arenaCenter;
            floor.transform.localScale = new Vector3(32.0f, 0.05f, 32.0f);
            SetMaterialColor(floor, new Color(0.25f, 0.15f, 0.18f)); // Dark Crimson Stone

            // 2. Arena Boundary Pillars & Fire Braziers (12 Pillars in a Ring)
            float radius = 16.0f;
            for (int i = 0; i < 12; i++)
            {
                float angle = i * (360f / 12f) * Mathf.Deg2Rad;
                Vector3 pillarPos = arenaCenter + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                // Stone Pillar
                GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pillar.name = $"Arena_Pillar_{i + 1}";
                pillar.transform.SetParent(_arenaRoot.transform);
                pillar.transform.position = pillarPos + new Vector3(0, 3.0f, 0);
                pillar.transform.localScale = new Vector3(1.6f, 6.0f, 1.6f);
                SetMaterialColor(pillar, new Color(0.15f, 0.12f, 0.14f));

                // Glowing Fire Torch on Pillar
                GameObject torch = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                torch.name = "Torch_Flame";
                torch.transform.SetParent(pillar.transform);
                torch.transform.localPosition = new Vector3(0, 0.55f, 0);
                torch.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                SetMaterialColor(torch, new Color(1.0f, 0.3f, 0.1f));
            }

            // 3. Spawn Goblin Chief Boss Gorgar
            SpawnBoss(arenaCenter);

            Debug.Log("<color=#FF0044>👑 [BossArenaGenerator]</color> Boss Arena & Gorgar the Goblin Chief successfully generated!");
        }

        public void ClearBossArena()
        {
            if (_arenaRoot != null)
            {
                Destroy(_arenaRoot);
                _arenaRoot = null;
            }

            GameObject oldRoot = GameObject.Find("[Goblin_Chief_Boss_Arena]");
            if (oldRoot != null)
            {
                Destroy(oldRoot);
            }
        }

        private void SpawnBoss(Vector3 center)
        {
            GameObject bossObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bossObj.name = "BOSS_Gorgar_Goblin_Chief";
            bossObj.transform.SetParent(_arenaRoot.transform);
            bossObj.transform.position = center + new Vector3(0, 1.3f, 4.0f);

            // Add Components
            bossObj.AddComponent<HealthSystem>();
            bossObj.AddComponent<GoblinChiefBoss>();
        }

        private void SetMaterialColor(GameObject obj, Color color)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.color = color;
            }
        }
    }
}
