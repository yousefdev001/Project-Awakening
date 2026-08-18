using Awakening.Combat;
using Awakening.Monsters;
using Awakening.World;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to test the Goblin Chief World Boss mechanics, Enrage mode, and Victory rewards.
    /// </summary>
    public class BossDebugControls : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            int boxW = 210;
            int boxH = 175;
            int boxX = 10;
            int boxY = 725;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "👑 Boss Encounter (Phase 24)");

            int btnY = boxY + 26;
            int btnH = 25;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, btnH), "👑 Teleport to Boss Arena"))
            {
                TeleportPlayer(new Vector3(0, 0.5f, 222.0f));
            }

            int btnY2 = btnY + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY2, 95, 24), "⚔️ Dmg 100"))
            {
                if (GoblinChiefBoss.Instance != null)
                {
                    HealthSystem hs = GoblinChiefBoss.Instance.GetComponent<HealthSystem>();
                    if (hs != null) hs.TakeDamage(new DamageData(100f, DamageType.True));
                }
            }

            if (GUI.Button(new Rect(boxX + 110, btnY2, 90, 24), "🔥 Enrage"))
            {
                if (GoblinChiefBoss.Instance != null)
                {
                    HealthSystem hs = GoblinChiefBoss.Instance.GetComponent<HealthSystem>();
                    if (hs != null && hs.CurrentHealth > 300f)
                    {
                        hs.TakeDamage(new DamageData(hs.CurrentHealth - 290f, DamageType.True));
                    }
                }
            }

            int btnY3 = btnY2 + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY3, boxW - 20, 24), "🏆 Slay Boss (Test Victory)"))
            {
                if (GoblinChiefBoss.Instance != null)
                {
                    HealthSystem hs = GoblinChiefBoss.Instance.GetComponent<HealthSystem>();
                    if (hs != null) hs.TakeDamage(new DamageData(9999f, DamageType.True));
                }
            }

            int btnY4 = btnY3 + 28;
            if (GUI.Button(new Rect(boxX + 10, btnY4, boxW - 20, 22), "🔄 Rebuild Boss Arena"))
            {
                if (BossArenaGenerator.Instance != null)
                {
                    BossArenaGenerator.Instance.BuildBossArena();
                }
            }
        }

        private void TeleportPlayer(Vector3 targetPos)
        {
            Transform player = Camera.main != null ? Camera.main.transform.root : null;
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.position = targetPos;
                if (cc != null) cc.enabled = true;
                Debug.Log($"<color=#00FFAA>[Teleport]</color> Teleported player to {targetPos}.");
            }
        }
    }
}
