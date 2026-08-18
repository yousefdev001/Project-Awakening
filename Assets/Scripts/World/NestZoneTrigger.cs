using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Displays a location banner when entering or exploring the Goblin Nest Dungeon.
    /// </summary>
    public class NestZoneTrigger : MonoBehaviour
    {
        [SerializeField] private string _zoneName = "The Goblin Nest";
        [SerializeField] private string _subTitle = "Dungeon Floor 1 (Lair of the Goblin Chief)";

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

            // Trigger when player enters Dungeon area (Z >= 120)
            if (_playerTransform.position.z >= 120f && !_hasTriggered)
            {
                _hasTriggered = true;
                TriggerBanner();
            }
            else if (_playerTransform.position.z < 110f && _hasTriggered)
            {
                _hasTriggered = false;
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
            int bannerW = 360;
            int bannerH = 52;
            int bannerX = (screenW - bannerW) / 2;
            int bannerY = 55;

            // Pitch dark background with deep purple/crimson outline
            GUI.color = new Color(0.08f, 0.02f, 0.04f, 0.92f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, bannerH), Texture2D.whiteTexture);

            GUI.color = new Color(0.9f, 0.15f, 0.2f, 0.95f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(bannerX, bannerY + bannerH - 2, bannerW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(bannerX, bannerY + 6, bannerW, 22), $"<size=13><b><color=#FF2244>👹 {_zoneName.ToUpper()}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(bannerX, bannerY + 26, bannerW, 18), $"<size=9><i><color=#FF8899>{_subTitle}</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
