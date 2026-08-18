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
            EnsureComponent<Awakening.VFX.VFXManager>();
            EnsureComponent<Awakening.Persistence.SaveSystem>();
            EnsureComponent<Awakening.Professions.AwakeningController>();
            EnsureComponent<Awakening.Professions.ProfessionRandomizer>();
            EnsureComponent<WorldZoneCoordinator>();
            EnsureComponent<VillageGenerator>();
            EnsureComponent<ForestGenerator>();
            EnsureComponent<NestGenerator>();
            EnsureComponent<BossArenaGenerator>();
        }

        private void EnsureUIHUDs()
        {
            EnsureComponent<MainMenuUI>();
            EnsureComponent<PauseMenuUI>();
            EnsureComponent<GameOverUI>();
            EnsureComponent<VictoryCreditsUI>();
            EnsureComponent<AwakeningScreenUI>();
            EnsureComponent<DialogueUI>();
            EnsureComponent<InteractionPromptUI>();
            EnsureComponent<QuestTrackerHUD>();
            EnsureComponent<BossHealthBarHUD>();
        }

        private void EnsureDebugPanels()
        {
            EnsureComponent<AudioDebugPanel>();
            EnsureComponent<VFXDebugPanel>();
            EnsureComponent<SaveDebugPanel>();
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
            EnsureComponent<AwakeningDebugTrigger>();
        }

        private void EnsurePlayerComponents()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                PlayerMovement existingMovement = FindFirstObjectByType<PlayerMovement>();
                if (existingMovement != null) playerObj = existingMovement.gameObject;
            }

            if (playerObj == null)
            {
                // Create Player GameObject if none exists in scene
                playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerObj.name = "Player";
                playerObj.tag = "Player";
                playerObj.transform.position = new Vector3(0, 0.5f, 2.0f);
            }

            // Ensure full player component stack
            EnsureComponentOnObj<CharacterController>(playerObj);
            EnsureComponentOnObj<PlayerMovement>(playerObj);
            EnsureComponentOnObj<PlayerStats>(playerObj);
            EnsureComponentOnObj<PlayerProgression>(playerObj);
            EnsureComponentOnObj<HealthSystem>(playerObj);
            EnsureComponentOnObj<HitboxDetector>(playerObj);
            EnsureComponentOnObj<PlayerCombat>(playerObj);
            EnsureComponentOnObj<PlayerAnimation>(playerObj);
            EnsureComponentOnObj<Awakening.Professions.ProfessionSystem>(playerObj);
            EnsureComponentOnObj<InventorySystem>(playerObj);
            EnsureComponentOnObj<Awakening.Equipment.EquipmentSystem>(playerObj);
            EnsureComponentOnObj<PlayerWallet>(playerObj);
            EnsureComponentOnObj<PlayerInteraction>(playerObj);
            EnsureComponentOnObj<QuestManager>(playerObj);
        }

        private T EnsureComponentOnObj<T>(GameObject target) where T : Component
        {
            T comp = target.GetComponent<T>();
            if (comp == null)
            {
                comp = target.AddComponent<T>();
            }
            return comp;
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
