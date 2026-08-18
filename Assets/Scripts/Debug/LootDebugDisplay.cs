using Awakening.Items;
using Awakening.Player;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to view Player Gold balance and spawn test world loot items.
    /// </summary>
    public class LootDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerWallet wallet = PlayerWallet.Instance;

            int boxW = 210;
            int boxH = 150;
            int boxX = Screen.width - 220;
            int boxY = 480;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "💰 Loot & Currency (Phase 15)");

            int currentGold = wallet != null ? wallet.Gold : 0;
            GUI.Label(new Rect(boxX + 10, boxY + 25, boxW - 20, 22), $"Gold Balance: <color=#FFD700><b>{currentGold} 🪙</b></color>");

            int btnY = boxY + 52;
            if (GUI.Button(new Rect(boxX + 10, btnY, 90, 24), "+100 Gold"))
            {
                if (wallet != null) wallet.AddGold(100);
            }

            if (GUI.Button(new Rect(boxX + 105, btnY, 95, 24), "-50 Gold"))
            {
                if (wallet != null) wallet.SpendGold(50);
            }

            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, boxW - 20, 24), "🧪 Drop Health Potion"))
            {
                SpawnTestItem(ItemData.CreateHealthPotionPreset());
            }

            if (GUI.Button(new Rect(boxX + 10, btnY2 + 26, boxW - 20, 24), "🐺 Drop Wolf Fur"))
            {
                SpawnTestItem(ItemData.CreateWolfFurPreset());
            }
        }

        private void SpawnTestItem(ItemData item)
        {
            Transform player = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = player != null ? player.forward : Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 spawnPos = (player != null ? player.position : Vector3.zero) + forward * 2.5f;
            spawnPos.y = 0.5f;

            GameObject pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickupObj.name = $"Drop_{item.itemName.Replace(" ", "_")}";
            pickupObj.transform.position = spawnPos;
            pickupObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            WorldItemPickup pickup = pickupObj.AddComponent<WorldItemPickup>();
            pickup.Setup(item, 1);

            Debug.Log($"<color=#00FFAA>[Loot]</color> Spawned test drop <b>{item.itemName}</b> at {spawnPos}!");
        }
    }
}
