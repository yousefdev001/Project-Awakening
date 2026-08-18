using Awakening.Equipment;
using Awakening.Inventory;
using Awakening.Items;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Interactive Inventory & Equipment UI screen.
    /// Draws a 20-slot bag grid, equipped gear slots (Weapon, Armor), Item details, Use, Equip, Drop, and Sort actions.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        private int _selectedSlotIndex = -1;

        private void OnGUI()
        {
            InventorySystem inv = InventorySystem.Instance;
            EquipmentSystem equip = EquipmentSystem.Instance;
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
            GUI.color = new Color(0f, 0f, 0f, 0.65f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Inventory Window Box (Expanded for Equipment)
            int winW = 680;
            int winH = 410;
            int winX = (screenW - winW) / 2;
            int winY = (screenH - winH) / 2;

            GUI.Box(new Rect(winX, winY, winW, winH), "🎒 PLAYER INVENTORY & EQUIPMENT (Phase 16 & 17) - [Tab / I]");

            // 1. Grid of 20 Slots (4 rows x 5 cols)
            int cols = 5;
            int slotW = 56;
            int slotH = 56;
            int startX = winX + 20;
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
                    GUI.DrawTexture(new Rect(slotX + 5, slotY + 5, slotW - 10, slotH - 10), Texture2D.whiteTexture);
                    GUI.color = Color.white;

                    string shortName = item.itemName.Length > 8 ? item.itemName.Substring(0, 7) + ".." : item.itemName;
                    GUI.Label(new Rect(slotX + 2, slotY + 3, slotW - 4, 18), $"<size=8><b>{shortName}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });

                    if (slot.Quantity > 1)
                    {
                        GUI.Label(new Rect(slotX + 2, slotY + slotH - 18, slotW - 6, 16), $"<size=9><b>x{slot.Quantity}</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight });
                    }
                }
            }

            // 2. Equipped Gear Section (Middle-Right Column)
            int equipX = winX + 335;
            int equipY = winY + 45;
            int equipW = 150;
            int equipH = 280;

            GUI.Box(new Rect(equipX, equipY, equipW, equipH), "⚔️ Equipped Gear");

            // Weapon Slot
            string wepName = (equip != null && equip.EquippedWeapon != null) ? equip.EquippedWeapon.itemName : "<i>None (Bare Hands)</i>";
            GUI.Label(new Rect(equipX + 8, equipY + 25, equipW - 16, 20), "<size=10><b>Main Weapon:</b></size>");
            GUI.Label(new Rect(equipX + 8, equipY + 45, equipW - 16, 20), $"<size=9><color=yellow>{wepName}</color></size>");
            if (equip != null && equip.EquippedWeapon != null)
            {
                if (GUI.Button(new Rect(equipX + 8, equipY + 68, equipW - 16, 20), "Unequip Weapon"))
                {
                    equip.UnequipToInventory(EquipmentSlotType.Weapon);
                }
            }

            // Armor Slot
            string armName = (equip != null && equip.EquippedArmor != null) ? equip.EquippedArmor.itemName : "<i>None (Cloth)</i>";
            GUI.Label(new Rect(equipX + 8, equipY + 95, equipW - 16, 20), "<size=10><b>Chest Armor:</b></size>");
            GUI.Label(new Rect(equipX + 8, equipY + 115, equipW - 16, 20), $"<size=9><color=cyan>{armName}</color></size>");
            if (equip != null && equip.EquippedArmor != null)
            {
                if (GUI.Button(new Rect(equipX + 8, equipY + 138, equipW - 16, 20), "Unequip Armor"))
                {
                    equip.UnequipToInventory(EquipmentSlotType.Armor);
                }
            }

            // Affinity Status Badge
            if (equip != null && equip.HasWeaponAffinity)
            {
                GUI.color = new Color(1f, 0.85f, 0.1f);
                GUI.Box(new Rect(equipX + 8, equipY + 175, equipW - 16, 50), "");
                GUI.color = Color.white;
                GUI.Label(new Rect(equipX + 10, equipY + 178, equipW - 20, 45), "<size=9><b>★ WEAPON AFFINITY ★\n<color=#00FFAA>+20% Synergy Boost</color>\n(Weapon matches class)</b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }

            // Quick Add Equipment for Testing
            if (GUI.Button(new Rect(equipX + 8, equipY + 235, 65, 22), "+Sword"))
            {
                inv.AddItem(ItemData.CreateIronLongswordPreset(), 1);
            }
            if (GUI.Button(new Rect(equipX + 76, equipY + 235, 65, 22), "+Bow"))
            {
                inv.AddItem(ItemData.CreateHunterBowPreset(), 1);
            }
            if (GUI.Button(new Rect(equipX + 8, equipY + 258, 65, 20), "+Staff"))
            {
                inv.AddItem(ItemData.CreateArcaneStaffPreset(), 1);
            }
            if (GUI.Button(new Rect(equipX + 76, equipY + 258, 65, 20), "+Armor"))
            {
                inv.AddItem(ItemData.CreateKnightArmorPreset(), 1);
            }

            // 3. Item Details & Actions (Far Right Column)
            int detailX = winX + 495;
            int detailY = winY + 45;
            int detailW = 165;
            int detailH = 280;

            GUI.Box(new Rect(detailX, detailY, detailW, detailH), "Item Details");

            InventorySlot selectedSlot = (_selectedSlotIndex >= 0 && _selectedSlotIndex < inv.Slots.Count)
                ? inv.Slots[_selectedSlotIndex]
                : null;

            if (selectedSlot != null && !selectedSlot.IsEmpty)
            {
                ItemData item = selectedSlot.Item;
                string hexColor = ColorUtility.ToHtmlStringRGB(item.themeColor);

                GUI.Label(new Rect(detailX + 8, detailY + 25, detailW - 16, 20), $"<size=11><b><color=#{hexColor}>{item.itemName}</color></b></size>");
                GUI.Label(new Rect(detailX + 8, detailY + 45, detailW - 16, 18), $"<size=9>[{item.rarity}] {item.itemType}</size>");
                GUI.Label(new Rect(detailX + 8, detailY + 63, detailW - 16, 18), $"<size=9>Qty: <b>x{selectedSlot.Quantity}</b> | {item.goldValue}🪙</size>");

                if (item.bonusAttack > 0 || item.bonusDefense > 0 || item.bonusMaxHealth > 0 || item.bonusMaxMana > 0)
                {
                    GUI.Label(new Rect(detailX + 8, detailY + 83, detailW - 16, 30), $"<size=9><b>Stats:</b> +{item.bonusAttack} Atk | +{item.bonusDefense} Def\n+{item.bonusMaxHealth} HP | +{item.bonusMaxMana} MP</size>");
                }

                GUI.Label(new Rect(detailX + 8, detailY + 115, detailW - 16, 50), $"<size=9><i>{item.description}</i></size>");

                // Action Buttons
                int btnY = detailY + 175;
                if (item.equipmentSlot != EquipmentSlotType.None)
                {
                    if (GUI.Button(new Rect(detailX + 8, btnY, detailW - 16, 26), "⚔️ EQUIP GEAR"))
                    {
                        if (equip != null)
                        {
                            equip.EquipFromInventory(_selectedSlotIndex);
                        }
                    }
                }
                else if (item.itemType == ItemType.Consumable)
                {
                    if (GUI.Button(new Rect(detailX + 8, btnY, detailW - 16, 26), "🧪 USE ITEM"))
                    {
                        inv.UseItem(_selectedSlotIndex);
                    }
                }

                if (GUI.Button(new Rect(detailX + 8, btnY + 30, detailW - 16, 24), "🗑️ DROP (1x)"))
                {
                    inv.DropItem(_selectedSlotIndex, 1);
                }

                if (selectedSlot.Quantity > 1)
                {
                    if (GUI.Button(new Rect(detailX + 8, btnY + 56, detailW - 16, 22), $"DROP ALL (x{selectedSlot.Quantity})"))
                    {
                        inv.DropItem(_selectedSlotIndex, selectedSlot.Quantity);
                    }
                }
            }
            else
            {
                GUI.Label(new Rect(detailX + 8, detailY + 90, detailW - 16, 40), "<size=10><i>Select an item slot to view details or equip.</i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }

            // Bottom Actions: Sort & Close
            if (GUI.Button(new Rect(winX + 20, winY + 345, 160, 34), "🔄 SORT INVENTORY"))
            {
                inv.SortInventory();
            }

            if (GUI.Button(new Rect(winX + winW - 150, winY + 345, 130, 34), "❌ CLOSE BAG"))
            {
                inv.CloseInventory();
            }
        }
    }
}
