using Awakening.VFX;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Interactive VFX Particle test board debug panel to preview all combat and world visual effects.
    /// </summary>
    public class VFXDebugPanel : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            VFXManager vm = VFXManager.Instance;
            if (vm == null) return;

            int boxW = 220;
            int boxH = 200;
            int boxX = Screen.width - 450;
            int boxY = 710;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "💥 VFX Particle Spawner (Phase 26)");

            Transform player = Camera.main != null ? Camera.main.transform.root : null;
            Vector3 pos = player != null ? player.position + Vector3.up * 0.8f : Vector3.zero;

            int btnY = boxY + 26;
            int btnW = 95;
            int btnH = 24;

            if (GUI.Button(new Rect(boxX + 10, btnY, btnW, btnH), "✨ Sparks")) vm.SpawnVFX(VFXType.SlashHitSparks, pos);
            if (GUI.Button(new Rect(boxX + 110, btnY, btnW, btnH), "🪓 Cleave")) vm.SpawnVFX(VFXType.HeavyCleaveBurst, pos);

            if (GUI.Button(new Rect(boxX + 10, btnY + 28, btnW, btnH), "🩸 Blood")) vm.SpawnVFX(VFXType.BloodSplatter, pos);
            if (GUI.Button(new Rect(boxX + 110, btnY + 28, btnW, btnH), "⚡ Magic")) vm.SpawnVFX(VFXType.SkillMagicBurst, pos);

            if (GUI.Button(new Rect(boxX + 10, btnY + 56, btnW, btnH), "💨 Dust")) vm.SpawnVFX(VFXType.DodgeDustTrail, player != null ? player.position : Vector3.zero);
            if (GUI.Button(new Rect(boxX + 110, btnY + 56, btnW, btnH), "💰 Sparkle")) vm.SpawnVFX(VFXType.GoldPickupSparkle, pos);

            if (GUI.Button(new Rect(boxX + 10, btnY + 84, btnW, btnH), "🌟 Pillar")) vm.SpawnVFX(VFXType.LevelUpPillar, player != null ? player.position : Vector3.zero);
            if (GUI.Button(new Rect(boxX + 110, btnY + 84, btnW, btnH), "💥 Slam")) vm.SpawnVFX(VFXType.GroundSlamShockwave, player != null ? player.position : Vector3.zero);

            if (GUI.Button(new Rect(boxX + 10, btnY + 112, boxW - 20, 24), "🔥 Boss Enrage Aura"))
            {
                vm.SpawnVFX(VFXType.BossEnrageAura, pos);
            }
        }
    }
}
