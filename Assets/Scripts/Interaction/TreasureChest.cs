using System.Collections.Generic;
using Awakening.Items;
using UnityEngine;

namespace Awakening.Interaction
{
    /// <summary>
    /// Interactive Treasure Chest in the 3D world.
    /// Opens on player interaction [F] and scatters valuable loot and gold coins.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TreasureChest : MonoBehaviour, IInteractable
    {
        [Header("Chest Configuration")]
        [SerializeField] private LootTable _lootTable;
        [SerializeField] private bool _isOpened = false;

        public string InteractionPrompt => _isOpened ? "Empty Chest" : "Open Treasure Chest";
        public bool IsOpened => _isOpened;

        private Renderer _renderer;
        private Color _originalColor = new Color(0.6f, 0.4f, 0.2f); // Wood brown

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null && _renderer.material != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        private void Start()
        {
            if (_lootTable == null)
            {
                // Create rich chest preset table
                _lootTable = ScriptableObject.CreateInstance<LootTable>();
                _lootTable.drops.Add(new LootDropEntry(ItemData.CreateGoldPreset(50), 100f, 1, 1));
                _lootTable.drops.Add(new LootDropEntry(ItemData.CreateHealthPotionPreset(), 100f, 2, 3));
                _lootTable.drops.Add(new LootDropEntry(ItemData.CreateManaPotionPreset(), 100f, 2, 3));
                _lootTable.drops.Add(new LootDropEntry(ItemData.CreateIronLongswordPreset(), 70f, 1, 1));
                _lootTable.drops.Add(new LootDropEntry(ItemData.CreateKnightArmorPreset(), 50f, 1, 1));
            }
        }

        public bool CanInteract(GameObject interactor)
        {
            return !_isOpened;
        }

        public void Interact(GameObject interactor)
        {
            if (_isOpened) return;
            _isOpened = true;

            // Visual open feedback: turn gold/yellow and scale lid
            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.color = new Color(1f, 0.84f, 0.2f); // Gold glow
            }

            Debug.Log("<color=#FFD700>[TreasureChest]</color> Chest opened! Bursting with treasures!");
            SpawnLootBurst();
        }

        private void SpawnLootBurst()
        {
            if (_lootTable == null) return;

            List<DroppedItemResult> drops = _lootTable.RollDrops();
            Vector3 origin = transform.position + Vector3.up * 0.8f;

            foreach (var drop in drops)
            {
                Vector2 randCircle = Random.insideUnitCircle * 2.0f;
                Vector3 spawnPos = origin + new Vector3(randCircle.x, 0f, randCircle.y);

                GameObject dropObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                dropObj.name = $"Loot_{drop.Item.itemName.Replace(" ", "_")}";
                dropObj.transform.position = spawnPos;
                dropObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

                WorldItemPickup pickup = dropObj.AddComponent<WorldItemPickup>();
                pickup.Setup(drop.Item, drop.Quantity);
            }
        }

        private void OnGUI()
        {
            if (Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * 1.4f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 20f)
            {
                float tagW = 140;
                float tagH = 20;
                float x = screenPos.x - (tagW / 2);
                float y = Screen.height - screenPos.y;

                string status = _isOpened ? "<color=grey>[Opened Chest]</color>" : "<color=#FFD700>📦 [Treasure Chest]</color>";
                GUI.Label(new Rect(x, y, tagW, tagH), $"<size=10><b>{status}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
