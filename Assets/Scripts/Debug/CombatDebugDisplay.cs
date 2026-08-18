using Awakening.Combat;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to monitor combat combos, damage numbers, and spawn test training dummies.
    /// </summary>
    public class CombatDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private string _lastCombatAction = "None";
        private float _lastActionDamage = 0f;

        private void Start()
        {
            if (PlayerCombat.Instance != null)
            {
                PlayerCombat.Instance.OnAttackExecuted += (combo, dmg) =>
                {
                    _lastCombatAction = $"Light Attack ({combo}/3)";
                    _lastActionDamage = dmg;
                };

                PlayerCombat.Instance.OnHeavyAttackExecuted += (dmg) =>
                {
                    _lastCombatAction = "Heavy Strike!";
                    _lastActionDamage = dmg;
                };

                PlayerCombat.Instance.OnDodgeExecuted += () =>
                {
                    _lastCombatAction = "Dodge (I-Frames)";
                    _lastActionDamage = 0f;
                };

                PlayerCombat.Instance.OnSkillExecuted += (skill, dmg) =>
                {
                    _lastCombatAction = $"Skill: {skill}";
                    _lastActionDamage = dmg;
                };
            }
        }

        private void OnGUI()
        {
            if (!_showUI) return;

            PlayerCombat combat = PlayerCombat.Instance;
            if (combat == null) return;

            int boxW = 260;
            int boxH = 175;
            int boxX = 10;
            int boxY = 240;

            GUI.Box(new Rect(boxX, boxY, boxW, boxH), "⚔️ Combat Engine (Phase 12)");

            GUI.Label(new Rect(boxX + 10, boxY + 25, boxW - 20, 20), $"Combo: <color=yellow><b>Step {combat.CurrentComboIndex}/3</b></color>");
            GUI.Label(new Rect(boxX + 10, boxY + 45, boxW - 20, 20), $"Last Action: <b>{_lastCombatAction}</b>");
            if (_lastActionDamage > 0f)
            {
                GUI.Label(new Rect(boxX + 10, boxY + 65, boxW - 20, 20), $"Damage Output: <color=#FF5555><b>{_lastActionDamage:F1} DMG</b></color>");
            }

            float cd = combat.SkillCooldownRemaining;
            string cdText = cd <= 0f ? "<color=#00FFAA>READY (Press E)</color>" : $"<color=orange>{cd:F1}s CD</color>";
            GUI.Label(new Rect(boxX + 10, boxY + 85, boxW - 20, 20), $"Skill Status: <b>{cdText}</b>");

            int btnY = boxY + 110;
            if (GUI.Button(new Rect(boxX + 10, btnY, boxW - 20, 26), "🎯 Spawn Training Dummy Target"))
            {
                SpawnTestDummy();
            }

            GUI.Label(new Rect(boxX + 10, btnY + 28, boxW - 20, 30), "<size=9>Controls: LMB=Combo | RMB=Heavy | Space=Dodge | E=Skill</size>");
        }

        private void SpawnTestDummy()
        {
            Transform player = PlayerCombat.Instance != null ? PlayerCombat.Instance.transform : null;
            Vector3 spawnPos = player != null ? player.position + player.forward * 3.0f : new Vector3(0, 1, 3);
            spawnPos.y = 1.0f;

            GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dummy.name = "Training_Dummy";
            dummy.transform.position = spawnPos;
            dummy.transform.localScale = new Vector3(0.9f, 1.0f, 0.9f);

            // Add HealthSystem & CombatDummyTarget
            dummy.AddComponent<HealthSystem>();
            dummy.AddComponent<CombatDummyTarget>();

            // Set Material Color to Orange
            Renderer rend = dummy.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(1f, 0.55f, 0.1f);
            }

            Debug.Log($"<color=#00FFAA>[Combat]</color> Spawned Training Dummy at {spawnPos}!");
        }
    }
}
