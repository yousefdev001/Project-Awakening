using Awakening.Interaction;
using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Interactive Magical Portal implementing IInteractable to teleport player between World Zones and Dungeons.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DungeonPortal : MonoBehaviour, IInteractable
    {
        [Header("Portal Configuration")]
        [SerializeField] private string _portalPrompt = "Enter Goblin Nest Dungeon";
        [SerializeField] private Vector3 _targetDestination = new Vector3(0, 0.5f, 130f);
        [SerializeField] private Color _runeColor = new Color(1f, 0.15f, 0.15f);

        public string InteractionPrompt => _portalPrompt;

        private void Start()
        {
            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.color = _runeColor;
            }
        }

        public void Setup(string prompt, Vector3 destination, Color color)
        {
            _portalPrompt = prompt;
            _targetDestination = destination;
            _runeColor = color;

            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.color = _runeColor;
            }
        }

        public bool CanInteract(GameObject interactor)
        {
            return true;
        }

        public void Interact(GameObject interactor)
        {
            CharacterController cc = interactor.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            interactor.transform.position = _targetDestination;

            if (cc != null) cc.enabled = true;

            Debug.Log($"<color=#00FFAA>[DungeonPortal]</color> Teleported through portal to {_targetDestination}!");
        }

        private void OnGUI()
        {
            if (Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * 1.8f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 22f)
            {
                float tagW = 160;
                float tagH = 20;
                float x = screenPos.x - (tagW / 2);
                float y = Screen.height - screenPos.y;

                string hexColor = ColorUtility.ToHtmlStringRGB(_runeColor);
                GUI.Label(new Rect(x, y, tagW, tagH), $"<size=10><b><color=#{hexColor}>🌀 [Dungeon Portal]</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
