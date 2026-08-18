using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Displays a location and danger alert banner when entering or exploring the Whispering Forest.
    /// </summary>
    public class ForestZoneTrigger : MonoBehaviour
    {
        [SerializeField] private string _zoneName = "Whispering Forest";
        [SerializeField] private string _subTitle = "Danger Level: Lv. 3 - 5 (Hostile Territory)";

        private bool _showBanner = false;
        private float _bannerTimer = 0f;
        private Transform _playerTransform;
        private bool _hasTriggered = false;

        private void Start()
        {
            Player.PlayerMovement player = FindFirstObjectByType<Player.PlayerMovement>();
            if (player != null) _playerTransform = player.transform;
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                Player.PlayerMovement player = FindFirstObjectByType<Player.PlayerMovement>();
                if (player != null) _playerTransform = player.transform;
                return;
            }

            // If player crosses north of Village Gate (Z > 22)
            if (_playerTransform.position.z >= 22f && !_hasTriggered)
            {
                _hasTriggered = true;
                TriggerBanner();
            }
            else if (_playerTransform.position.z < 18f && _hasTriggered)
            {
                _hasTriggered = false; // Reset when returning to village
            }

            if (_bannerTimer > 0f)
            {
                _bannerTimer -= Time.deltaTime;
                if (_bannerTimer <= 0f)
                {
                    _showBanner = false;
                }
            }
        }

        public void TriggerBanner()
        {
            _showBanner = true;
            _bannerTimer = 4.0f;
        }

        private void OnGUI()
        {
            if (!_showBanner) return;

            int screenW = Screen.width;
            int bannerW = 340;
            int bannerH = 50;
            int bannerX = (screenW - bannerW) / 2;
            int bannerY = 55;

            // Crimson-tinted dark background for danger zone
            GUI.color = new Color(0.12f, 0.04f, 0.04f, 0.85f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, bannerH), Texture2D.whiteTexture);

            // Red/Orange danger border
            GUI.color = new Color(1f, 0.35f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(bannerX, bannerY + bannerH - 2, bannerW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(bannerX, bannerY + 6, bannerW, 22), $"<size=13><b><color=#FF5555>🌲 {_zoneName.ToUpper()}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(bannerX, bannerY + 26, bannerW, 18), $"<size=9><i><color=#FFAA77>{_subTitle}</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
