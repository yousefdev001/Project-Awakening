using Awakening.Combat;
using Awakening.GameDebug;
using Awakening.GameUI;
using Awakening.Interaction;
using Awakening.Inventory;
using Awakening.Player;
using Awakening.Quests;
using Awakening.World;
using UnityEngine;

namespace Awakening.Core
{
    /// <summary>
    /// Master Game Auto-Bootstrap.
    /// Automatically detects and adds all necessary Managers, Generators, UI HUDs, and Debug tools at runtime,
    /// removing the need to manually search and attach 15+ components in the Unity Inspector!
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureWorldGenerators();
            EnsureUIHUDs();
            EnsureDebugPanels();
            EnsurePlayerComponents();

            Debug.Log("<color=#00FFAA>🚀 [GameBootstrap]</color> All Core, World, UI, and Debug systems automatically verified and mounted!");
        }

        private void EnsureWorldGenerators()
        {
            EnsureComponent<Awakening.Audio.AudioManager>();
            EnsureComponent<WorldZoneCoordinator>();
            EnsureComponent<VillageGenerator>();
            EnsureComponent<ForestGenerator>();
            EnsureComponent<NestGenerator>();
            EnsureComponent<BossArenaGenerator>();
        }

        private void EnsureUIHUDs()
        {
            EnsureComponent<DialogueUI>();
            EnsureComponent<InteractionPromptUI>();
            EnsureComponent<QuestTrackerHUD>();
            EnsureComponent<BossHealthBarHUD>();
        }

        private void EnsureDebugPanels()
        {
            EnsureComponent<AudioDebugPanel>();
            EnsureComponent<VillageDebugControls>();
            EnsureComponent<ForestDebugControls>();
            EnsureComponent<NestDebugControls>();
            EnsureComponent<BossDebugControls>();
            EnsureComponent<QuestDebugDisplay>();
            EnsureComponent<InteractionDebugSpawner>();
            EnsureComponent<NPCSpawnerDebug>();
            EnsureComponent<MonsterSpawnerDebug>();
            EnsureComponent<CombatDebugDisplay>();
            EnsureComponent<LootDebugDisplay>();
        }

        private void EnsurePlayerComponents()
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                if (player.GetComponent<PlayerInteraction>() == null)
                {
                    player.gameObject.AddComponent<PlayerInteraction>();
                }

                if (player.GetComponent<QuestManager>() == null)
                {
                    player.gameObject.AddComponent<QuestManager>();
                }
            }
        }

        private T EnsureComponent<T>() where T : Component
        {
            T comp = GetComponent<T>();
            if (comp == null)
            {
                comp = gameObject.AddComponent<T>();
            }
            return comp;
        }
    }
}
