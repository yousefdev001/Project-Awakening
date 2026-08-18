using UnityEngine;

namespace Awakening.World
{
    /// <summary>
    /// Displays a location banner when entering or roaming within the Village Safe Zone.
    /// </summary>
    public class VillageZoneTrigger : MonoBehaviour
    {
        [SerializeField] private string _zoneName = "Oakhaven Village";
        [SerializeField] private string _subTitle = "Safe Haven of the Awakened";

        private bool _showBanner = true;
        private float _bannerTimer = 4.0f;

        private void Start()
        {
            _showBanner = true;
            _bannerTimer = 4.0f;
        }

        private void Update()
        {
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
            int bannerW = 320;
            int bannerH = 50;
            int bannerX = (screenW - bannerW) / 2;
            int bannerY = 55;

            // Semi-transparent background
            GUI.color = new Color(0.05f, 0.08f, 0.12f, 0.8f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, bannerH), Texture2D.whiteTexture);

            // Gold accent lines
            GUI.color = new Color(1f, 0.85f, 0.2f, 0.9f);
            GUI.DrawTexture(new Rect(bannerX, bannerY, bannerW, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(bannerX, bannerY + bannerH - 2, bannerW, 2), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(bannerX, bannerY + 6, bannerW, 22), $"<size=13><b><color=#FFD700>🛡️ {_zoneName.ToUpper()}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            GUI.Label(new Rect(bannerX, bannerY + 26, bannerW, 18), $"<size=9><i><color=#DDD>{_subTitle}</color></i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
