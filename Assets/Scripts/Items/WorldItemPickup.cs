using Awakening.Player;
using UnityEngine;

namespace Awakening.Items
{
    /// <summary>
    /// Interactive physical 3D drop item in the world.
    /// Features floating name tags, gentle rotation/bobbing, and magnetic collection towards the player.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WorldItemPickup : MonoBehaviour
    {
        [Header("Item Payload")]
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity = 1;

        [Header("Pickup Magnetics")]
        [SerializeField] private float _vacuumRadius = 3.5f;
        [SerializeField] private float _pickupRadius = 0.8f;
        [SerializeField] private float _vacuumSpeed = 8.0f;

        public ItemData Item => _itemData;
        public int Quantity => _quantity;

        private Transform _playerTransform;
        private Vector3 _basePosition;
        private bool _isBeingCollected = false;

        private void Start()
        {
            _basePosition = transform.position;

            PlayerMovement playerMov = FindFirstObjectByType<PlayerMovement>();
            if (playerMov != null)
            {
                _playerTransform = playerMov.transform;
            }

            // Set Renderer color if available
            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null && _itemData != null)
            {
                rend.material.color = _itemData.themeColor;
            }
        }

        public void Setup(ItemData item, int quantity)
        {
            _itemData = item;
            _quantity = quantity;

            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null && _itemData != null)
            {
                rend.material.color = _itemData.themeColor;
            }
        }

        private void Update()
        {
            if (_isBeingCollected) return;

            // Idle Bob & Spin
            transform.Rotate(Vector3.up, 60.0f * Time.deltaTime, Space.World);

            if (_playerTransform == null)
            {
                PlayerMovement playerMov = FindFirstObjectByType<PlayerMovement>();
                if (playerMov != null) _playerTransform = playerMov.transform;
                return;
            }

            float distToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Magnetic vacuum towards player
            if (distToPlayer <= _vacuumRadius)
            {
                transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position + Vector3.up * 0.8f, _vacuumSpeed * Time.deltaTime);

                if (distToPlayer <= _pickupRadius)
                {
                    CollectItem();
                }
            }
        }

        private void CollectItem()
        {
            if (_isBeingCollected) return;
            _isBeingCollected = true;

            if (_itemData != null)
            {
                if (_itemData.itemType == ItemType.Gold)
                {
                    int totalGold = _itemData.goldValue * _quantity;
                    if (PlayerWallet.Instance != null)
                    {
                        PlayerWallet.Instance.AddGold(totalGold);
                    }
                }
                else
                {
                    Debug.Log($"<color=#00FFAA>[Loot Pickup]</color> Collected <b>{_quantity}x [{_itemData.rarity}] {_itemData.itemName}</b>!");
                }
            }

            Destroy(gameObject);
        }

        private void OnGUI()
        {
            if (_itemData == null || Camera.main == null) return;

            Vector3 worldPos = transform.position + Vector3.up * 0.6f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0 && screenPos.z < 20f)
            {
                float tagW = 120;
                float tagH = 18;
                float x = screenPos.x - (tagW / 2);
                float y = Screen.height - screenPos.y;

                string hexColor = ColorUtility.ToHtmlStringRGB(_itemData.themeColor);
                string qtyText = _quantity > 1 ? $" x{_quantity}" : "";

                GUI.color = new Color(0f, 0f, 0f, 0.7f);
                GUI.DrawTexture(new Rect(x - 4, y - 2, tagW + 8, tagH + 4), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.Label(new Rect(x, y, tagW, tagH), $"<size=9><b><color=#{hexColor}>{_itemData.itemName}{qtyText}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
