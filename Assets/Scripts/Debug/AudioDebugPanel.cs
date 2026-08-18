using Awakening.Audio;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Interactive Audio Sound Board debug panel to test all synthesized sound effects and volume sliders.
    /// </summary>
    public class AudioDebugPanel : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            AudioManager am = AudioManager.Instance;
            if (am == null) return;

            int boxW = 220;
            int boxH = 220;
            int boxX = Screen.width - 450;
            int boxY = 480;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "🎵 Audio Soundboard (Phase 25)");

            // Volume Slider
            GUI.Label(new Rect(boxX + 10, boxY + 24, 80, 20), $"SFX: {(am.SFXVolume * 100):F0}%");
            am.SFXVolume = GUI.HorizontalSlider(new Rect(boxX + 90, boxY + 28, boxW - 100, 15), am.SFXVolume, 0f, 1f);

            int btnY = boxY + 48;
            int btnW = 95;
            int btnH = 24;

            // Combat sounds
            if (GUI.Button(new Rect(boxX + 10, btnY, btnW, btnH), "⚔️ Slash")) am.PlaySound(SoundType.AttackSlash);
            if (GUI.Button(new Rect(boxX + 110, btnY, btnW, btnH), "🪓 Cleave")) am.PlaySound(SoundType.HeavyCleave);

            if (GUI.Button(new Rect(boxX + 10, btnY + 28, btnW, btnH), "💨 Dodge")) am.PlaySound(SoundType.DodgeWhoosh);
            if (GUI.Button(new Rect(boxX + 110, btnY + 28, btnW, btnH), "⚡ Skill")) am.PlaySound(SoundType.SkillCast);

            if (GUI.Button(new Rect(boxX + 10, btnY + 56, btnW, btnH), "💥 Slam")) am.PlaySound(SoundType.GroundSlam);
            if (GUI.Button(new Rect(boxX + 110, btnY + 56, btnW, btnH), "💰 Gold")) am.PlaySound(SoundType.GoldChink);

            if (GUI.Button(new Rect(boxX + 10, btnY + 84, btnW, btnH), "🧪 Potion")) am.PlaySound(SoundType.PotionDrink);
            if (GUI.Button(new Rect(boxX + 110, btnY + 84, btnW, btnH), "📦 Chest")) am.PlaySound(SoundType.ChestOpen);

            if (GUI.Button(new Rect(boxX + 10, btnY + 112, btnW, btnH), "🌟 Level Up")) am.PlaySound(SoundType.LevelUpFanfare);
            if (GUI.Button(new Rect(boxX + 110, btnY + 112, btnW, btnH), "👑 Victory")) am.PlaySound(SoundType.VictoryFanfare);

            if (GUI.Button(new Rect(boxX + 10, btnY + 140, boxW - 20, 22), "👹 Boss Roar & Enrage"))
            {
                am.PlaySound(SoundType.BossRoar);
            }
        }
    }
}
