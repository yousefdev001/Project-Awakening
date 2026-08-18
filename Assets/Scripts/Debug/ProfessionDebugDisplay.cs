using Awakening.Professions;
using UnityEngine;

namespace Awakening.GameDebug
{
    /// <summary>
    /// Debug panel to test assigning and switching professions in Play Mode.
    /// </summary>
    public class ProfessionDebugDisplay : MonoBehaviour
    {
        [SerializeField] private bool _showUI = true;

        private void OnGUI()
        {
            if (!_showUI) return;

            ProfessionSystem sys = ProfessionSystem.Instance;
            if (sys == null) return;

            int boxWidth = 260;
            int boxHeight = 220;
            int boxX = Screen.width - 270;
            int boxY = Screen.height - 230;

            GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "🌟 Profession System (Phase 9)");

            if (sys.HasProfession)
            {
                ProfessionData data = sys.CurrentProfession;
                GUI.Label(new Rect(boxX + 10, boxY + 25, boxWidth - 20, 22), $"Rank: <b><color=yellow>{data.rank}</color></b> | <b>{data.professionName}</b>");
                GUI.Label(new Rect(boxX + 10, boxY + 45, boxWidth - 20, 22), $"Weapon: <b>{data.weaponAffinity}</b> | Skill: <i>{data.skillName}</i>");
                GUI.Label(new Rect(boxX + 10, boxY + 65, boxWidth - 20, 35), $"<size=9>Bonuses: +{data.bonusMaxHealth} HP | +{data.bonusAttack} Atk | +{data.bonusDefense} Def | +{data.bonusSpeed} Spd</size>");
            }
            else
            {
                GUI.Label(new Rect(boxX + 10, boxY + 30, boxWidth - 20, 25), "Status: <color=grey>Unawakened (No Profession)</color>");
            }

            int btnY = boxY + 105;
            int btnH = 24;

            if (GUI.Button(new Rect(boxX + 10, btnY, boxWidth - 20, btnH), "⚔️ Swordsman (Rank C)"))
            {
                sys.AssignProfession(ProfessionSystem.CreateSwordsmanPreset());
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 28, boxWidth - 20, btnH), "🏹 Hunter (Rank B)"))
            {
                sys.AssignProfession(ProfessionSystem.CreateHunterPreset());
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 56, boxWidth - 20, btnH), "🪄 Battle Mage (Rank A)"))
            {
                sys.AssignProfession(ProfessionSystem.CreateBattleMagePreset());
            }

            if (GUI.Button(new Rect(boxX + 10, btnY + 84, boxWidth - 20, 20), "Remove Profession"))
            {
                sys.RemoveProfession();
            }
        }
    }
}
