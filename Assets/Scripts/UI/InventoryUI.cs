using Awakening.Inventory;
using Awakening.Items;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Interactive Inventory UI screen.
    /// Draws a 20-slot grid, item details preview, Use, Drop, and Sort actions.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        private int _selectedSlotIndex = -1;

        private void OnGUI()
        {
            InventorySystem inv = InventorySystem.Instance;
            if (inv == null) return;

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Quick Toggle Button at Top-Left
            if (GUI.Button(new Rect(10, 10, 160, 28), inv.IsOpen ? "❌ Close Bag (Tab)" : "🎒 Open Bag (Tab / I)"))
            {
                inv.ToggleInventory();
            }

            if (!inv.IsOpen) return;

            // Semi-transparent overlay
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Inventory Window Box
            int winW = 540;
            int winH = 380;
            int winX = (screenW - winW) / 2;
            int winY = (screenH - winH) / 2;

            GUI.Box(new Rect(winX, winY, winW, winH), "🎒 PLAYER INVENTORY (20 SLOTS) - [Tab / I]");

            // 1. Grid of 20 Slots (4 rows x 5 cols)
            int cols = 5;
            int rows = 4;
            int slotW = 58;
            int slotH = 58;
            int startX = winX + 25;
            int startY = winY + 45;
            int spacing = 6;

            for (int i = 0; i < inv.Capacity; i++)
            {
                int c = i % cols;
                int r = i / cols;
                int slotX = startX + c * (slotW + spacing);
                int slotY = startY + r * (slotH + spacing);

                Rect slotRect = new Rect(slotX, slotY, slotW, slotH);
                InventorySlot slot = (i < inv.Slots.Count) ? inv.Slots[i] : null;

                // Highlight selected slot
                if (i == _selectedSlotIndex)
                {
                    GUI.color = new Color(1f, 0.9f, 0.2f);
                    GUI.Box(new Rect(slotX - 2, slotY - 2, slotW + 4, slotH + 4), "");
                    GUI.color = Color.white;
                }

                if (GUI.Button(slotRect, ""))
                {
                    _selectedSlotIndex = i;
                }

                if (slot != null && !slot.IsEmpty)
                {
                    ItemData item = slot.Item;
                    GUI.color = item.themeColor;
                    GUI.DrawTexture(new Rect(slotX + 6, slotY + 6, slotW - 12, slotH - 12), Texture2D.whiteTexture);
                    GUI.color = Color.white;

                    // Short Name
                    string shortName = item.itemName.Length > 8 ? item.itemName.Substring(0, 7) + ".." : item.itemName;
                    GUI.Label(new Rect(slotX + 2, slotY + 4, slotW - 4, 18), $"<size=8><b>{shortName}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

                    // Quantity tag
                    if (slot.Quantity > 1)
                    {
                        GUI.Label(new Rect(slotX + 2, slotY + slotH - 18, slotW - 6, 16), $"<size=9><b>x{slot.Quantity}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight });
                    }
                }
            }

            // 2. Details & Action Pane (Right column)
            int detailX = winX + 345;
            int detailY = winY + 45;
            int detailW = 175;
            int detailH = 250;

            GUI.Box(new Rect(detailX, detailY, detailW, detailH), "Item Details");

            InventorySlot selectedSlot = (_selectedSlotIndex >= 0 && _selectedSlotIndex < inv.Slots.Count)
                ? inv.Slots[_selectedSlotIndex]
                : null;

            if (selectedSlot != null && !selectedSlot.IsEmpty)
            {
                ItemData item = selectedSlot.Item;
                string hexColor = ColorUtility.ToHtmlStringRGB(item.themeColor);

                GUI.Label(new Rect(detailX + 10, detailY + 25, detailW - 20, 22), $"<size=11><b><color=#{hexColor}>{item.itemName}</color></b></size>");
                GUI.Label(new Rect(detailX + 10, detailY + 45, detailW - 20, 18), $"<size=9>Type: {item.itemType} | [{item.rarity}]</size>");
                GUI.Label(new Rect(detailX + 10, detailY + 63, detailW - 20, 18), $"<size=9>Quantity: <b>x{selectedSlot.Quantity}</b> | Value: {item.goldValue}🪙</size>");

                GUI.Label(new Rect(detailX + 10, detailY + 85, detailW - 20, 60), $"<size=9><i>{item.description}</i></size>");

                // Action Buttons
                int btnY = detailY + 155;
                if (item.itemType == ItemType.Consumable)
                {
                    if (GUI.Button(new Rect(detailX + 10, btnY, detailW - 20, 26), "🧪 USE ITEM"))
                    {
                        inv.UseItem(_selectedSlotIndex);
                    }
                }

                if (GUI.Button(new Rect(detailX + 10, btnY + 30, detailW - 20, 24), "🗑️ DROP (1x)"))
                {
                    inv.DropItem(_selectedSlotIndex, 1);
                }

                if (selectedSlot.Quantity > 1)
                {
                    if (GUI.Button(new Rect(detailX + 10, btnY + 56, detailW - 20, 22), $"DROP ALL (x{selectedSlot.Quantity})"))
                    {
                        inv.DropItem(_selectedSlotIndex, selectedSlot.Quantity);
                    }
                }
            }
            else
            {
                GUI.Label(new Rect(detailX + 10, detailY + 90, detailW - 20, 40), "<size=10><i>Select an item slot to view details or use.</i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }

            // Bottom Actions: Sort & Close
            if (GUI.Button(new Rect(winX + 25, winY + 315, 150, 32), "🔄 SORT INVENTORY"))
            {
                inv.SortInventory();
            }

            if (GUI.Button(new Rect(winX + winW - 145, winY + 315, 120, 32), "❌ CLOSE"))
            {
                inv.CloseInventory();
            }
        }
    }
}
