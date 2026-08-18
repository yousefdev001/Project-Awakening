using Awakening.GameUI;
using Awakening.Interaction;
using UnityEngine;

namespace Awakening.NPCs
{
    /// <summary>
    /// Interactive NPC Controller in the world. Implements IInteractable to trigger story dialogue and services.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NPCController : MonoBehaviour, IInteractable
    {
        [Header("NPC Data Profile")]
        [SerializeField] private NPCData _data;

        public NPCData Data => _data;
        public string InteractionPrompt => _data != null ? $"Talk to {_data.npcName} ({_data.npcRole})" : "Talk to NPC";

        private Transform _playerTransform;
        private Renderer _renderer;
        private bool _isInteracting = false;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();

            if (_data == null)
            {
                _data = NPCData.CreateElderPreset();
            }

            ApplyVisuals();
        }

        public void SetNPCData(NPCData data)
        {
            _data = data;
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            if (_data == null) return;

            if (_renderer != null && _renderer.material != null)
            {
                _renderer.material.color = _data.themeColor;
            }
        }

        public bool CanInteract(GameObject interactor)
        {
            return true;
        }

        public void Interact(GameObject interactor)
        {
            _playerTransform = interactor.transform;
            _isInteracting = true;

            // Trigger Dialogue UI
            if (DialogueUI.Instance != null && _data != null)
            {
                DialogueUI.Instance.StartDialogue(_data, () =>
                {
                    _isInteracting = false;
                });
            }
            else
            {
                Debug.Log($"<color=#00D4FF>[NPC: {_data?.npcName}]</color> \"{_data?.greeting}\"");
            }
        }

        private void Update()
        {
            // Face player while interacting
            if (_isInteracting && _playerTransform != null)
            {
                Vector3 lookDir = (_playerTransform.position - transform.position);
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8.0f);
                }
            }
        }

        private void OnGUI()
        {
            if (_data == null || Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * 2.2f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 22f)
            {
                float tagW = 160;
                float x = screenPos.x - (tagW / 2);
                float y = Screen.height - screenPos.y;

                string hexColor = ColorUtility.ToHtmlStringRGB(_data.themeColor);

                // NPC Name & Title
                GUI.Label(new Rect(x, y, tagW, 18), $"<size=11><b><color=#{hexColor}>{_data.npcName}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                GUI.Label(new Rect(x, y + 16, tagW, 16), $"<size=9><i><color=#DDD>{_data.npcRole}</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
