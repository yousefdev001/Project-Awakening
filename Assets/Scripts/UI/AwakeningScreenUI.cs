using Awakening.Professions;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Cinematic Fullscreen Awakening UI screen.
    /// Visualizes the Energy Analysis, Rank Reveal flash, and Profession details.
    /// </summary>
    public class AwakeningScreenUI : MonoBehaviour
    {
        private float _analysisProgress = 0f;

        private void Start()
        {
            if (AwakeningController.Instance != null)
            {
                AwakeningController.Instance.OnAnalysisProgress += p => _analysisProgress = p;
            }
        }

        private void OnGUI()
        {
            AwakeningController controller = AwakeningController.Instance;
            if (controller == null || !controller.IsAwakening) return;

            int screenW = Screen.width;
            int screenH = Screen.height;

            // 1. Dark Cinematic Backdrop
            GUI.color = new Color(0f, 0f, 0f, 0.85f);
            GUI.DrawTexture(new Rect(0, 0, screenW, screenH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Center Panel
            int panelW = 480;
            int panelH = 360;
            int panelX = (screenW - panelW) / 2;
            int panelY = (screenH - panelH) / 2;

            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "✨ THE SACRED AWAKENING RITE ✨");

            // Step 1: Energy Analysis
            if (controller.CurrentStep == AwakeningStep.Analyzing)
            {
                GUI.Label(new Rect(panelX + 20, panelY + 70, panelW - 40, 30), "<size=16><b>Analyzing Soul Potential & Mana Affinity...</b></size>");
                
                // Progress Bar
                int barW = panelW - 60;
                int barH = 26;
                int barX = panelX + 30;
                int barY = panelY + 130;

                GUI.Box(new Rect(barX, barY, barW, barH), "");
                GUI.color = new Color(0f, 0.8f, 1f, 0.9f);
                GUI.DrawTexture(new Rect(barX + 2, barY + 2, (barW - 4) * _analysisProgress, barH - 4), Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.Label(new Rect(barX, barY + 3, barW, barH), $"<color=black><b>{(_analysisProgress * 100f):F0}%</b></color>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

                GUI.Label(new Rect(panelX + 20, panelY + 200, panelW - 40, 30), "<size=12><i>The Ancient Circle channels ancestral power into your vessel...</i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
            // Step 2: Rank Revealed Flash
            else if (controller.CurrentStep == AwakeningStep.RankRevealed)
            {
                ProfessionData data = controller.RolledProfession;
                string hexColor = ColorUtility.ToHtmlStringRGB(data.rankColor);

                GUI.Label(new Rect(panelX + 20, panelY + 50, panelW - 40, 30), "<size=14>RANK ASSESSMENT COMPLETE</size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                GUI.Label(new Rect(panelX + 20, panelY + 110, panelW - 40, 70), $"<size=44><b><color=#{hexColor}>RANK {data.rank.ToString().Replace("Rank", "")}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                GUI.Label(new Rect(panelX + 20, panelY + 210, panelW - 40, 30), "<size=13><i>Manifesting Profession Archetype...</i></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
            }
            // Step 3: Profession Unveiled
            else if (controller.CurrentStep == AwakeningStep.ProfessionRevealed)
            {
                ProfessionData data = controller.RolledProfession;
                string hexColor = ColorUtility.ToHtmlStringRGB(data.rankColor);

                GUI.Label(new Rect(panelX + 20, panelY + 35, panelW - 40, 30), $"<size=18><b><color=#{hexColor}>[{data.rank}] {data.professionName.ToUpper()}</color></b></size>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

                GUI.Label(new Rect(panelX + 30, panelY + 75, panelW - 60, 45), $"<size=11>{data.description}</size>");

                // Combat Attributes
                GUI.Label(new Rect(panelX + 30, panelY + 125, panelW - 60, 25), $"<b>Favored Weapon:</b> <color=yellow>{data.weaponAffinity}</color>");
                GUI.Label(new Rect(panelX + 30, panelY + 150, panelW - 60, 25), $"<b>Primary Skill:</b> <color=#00FFAA>{data.skillName}</color> ({data.skillDescription})");

                // Stat Modifiers
                GUI.Label(new Rect(panelX + 30, panelY + 185, panelW - 60, 40), 
                    $"<size=11><b>Stat Growth:</b> +{data.bonusVitality} Vitality | +{data.bonusIntelligence} Intelligence\n" +
                    $"+{data.bonusMaxHealth} HP | +{data.bonusMaxMana} MP | +{data.bonusAttack} Atk | +{data.bonusDefense} Def</size>");

                // Embrace Button
                int btnW = 260;
                int btnH = 40;
                int btnX = panelX + (panelW - btnW) / 2;
                int btnY = panelY + 275;

                if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "✨ EMBRACE AWAKENING ✨"))
                {
                    controller.ConfirmAndFinishAwakening();
                }
            }
        }
    }
}
