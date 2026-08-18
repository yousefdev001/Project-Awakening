using Awakening.Player;
using UnityEngine;

namespace Awakening.Interaction
{
    /// <summary>
    /// Interactive Campfire providing the player with full Health and Mana restoration on [F] interaction.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CampfireRestPoint : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => "Rest at Campfire (Restore HP & MP)";

        public bool CanInteract(GameObject interactor)
        {
            return true;
        }

        public void Interact(GameObject interactor)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.Heal(PlayerStats.Instance.MaxHealth);
                PlayerStats.Instance.RestoreMana(PlayerStats.Instance.MaxMana);
                Debug.Log("<color=#FF8800>🔥 [Campfire Rest]</color> The warm embers soothe your soul. <b>HP & MP Fully Restored!</b>");
            }
        }

        private void OnGUI()
        {
            if (Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * 1.2f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 20f)
            {
                float tagW = 150;
                float tagH = 20;
                float x = screenPos.x - (tagW / 2);
                float y = Screen.height - screenPos.y;

                GUI.Label(new Rect(x, y, tagW, tagH), "<size=10><b><color=#FF7700>🔥 [Campfire Rest Site]</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
