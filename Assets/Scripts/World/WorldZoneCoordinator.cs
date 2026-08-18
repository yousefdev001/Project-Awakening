using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Master coordinator managing the entire world layout (Village, Whispering Forest, and Nest Dungeons).
    /// </summary>
    public class WorldZoneCoordinator : MonoBehaviour
    {
        public static WorldZoneCoordinator Instance { get; private set; }

        [Header("World Zone Sub-Generators")]
        [SerializeField] private bool _generateWorldOnAwake = true;

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
            if (_generateWorldOnAwake)
            {
                BuildFullWorld();
            }
        }

        public void BuildFullWorld()
        {
            // 1. Build Village
            if (VillageGenerator.Instance != null)
            {
                VillageGenerator.Instance.BuildVillage();
            }

            // 2. Build Whispering Forest
            if (ForestGenerator.Instance != null)
            {
                ForestGenerator.Instance.BuildForest();
            }

            Debug.Log("<color=#00FFAA>🌍 [WorldZoneCoordinator]</color> Complete Project Awakening MVP World generated!");
        }

        public void ClearFullWorld()
        {
            if (VillageGenerator.Instance != null) VillageGenerator.Instance.ClearVillage();
            if (ForestGenerator.Instance != null) ForestGenerator.Instance.ClearForest();
        }
    }
}
