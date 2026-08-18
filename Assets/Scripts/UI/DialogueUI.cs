using System;
using Awakening.NPCs;
using UnityEngine;

namespace Awakening.GameUI
{
    /// <summary>
    /// Interactive RPG Dialogue Box UI screen.
    /// Manages multi-stage NPC dialogues, character greetings, and conversation flow.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        public bool IsInDialogue { get; private set; } = false;

        private NPCData _currentNPC;
        private int _currentLineIndex = -1; // -1 = Greeting, 0+ = dialogueLines
        private Action _onDialogueComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void StartDialogue(NPCData data, Action onComplete = null)
        {
            if (data == null) return;

            _currentNPC = data;
            _currentLineIndex = -1; // Start at Greeting
            _onDialogueComplete = onComplete;
            IsInDialogue = true;

            Debug.Log($"<color=#00D4FF>[Dialogue]</color> Conversation started with <b>{_currentNPC.npcName}</b>.");
        }

        public void AdvanceDialogue()
        {
            if (_currentNPC == null) return;

            _currentLineIndex++;

            if (_currentNPC.dialogueLines == null || _currentLineIndex >= _currentNPC.dialogueLines.Length)
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            IsInDialogue = false;
            _currentNPC = null;
            _currentLineIndex = -1;

            _onDialogueComplete?.Invoke();
            _onDialogueComplete = null;

            Debug.Log("<color=#00D4FF>[Dialogue]</color> Conversation ended.");
        }

        private void OnGUI()
        {
            if (!IsInDialogue || _currentNPC == null) return;

            int screenW = Screen.width;
            int screenH = Screen.height;

            // Dialogue Box at bottom of screen
            int boxW = Mathf.Min(640, screenW - 40);
            int boxH = 160;
            int boxX = (screenW - boxW) / 2;
            int boxY = screenH - boxH - 25;

            // Dark semi-transparent background
            GUI.color = new Color(0.06f, 0.09f, 0.14f, 0.92f);
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, boxH), Texture2D.whiteTexture);

            // NPC Theme Color Top Accent Border
            GUI.color = _currentNPC.themeColor;
            GUI.DrawTexture(new Rect(boxX, boxY, boxW, 3), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // NPC Name and Role Header
            string hexColor = ColorUtility.ToHtmlStringRGB(_currentNPC.themeColor);
            GUI.Label(new Rect(boxX + 20, boxY + 12, boxW - 40, 24), $"<size=13><b><color=#{hexColor}>{_currentNPC.npcName}</color></b> — <i><color=#BBB>{_currentNPC.npcRole}</color></i></size>");

            // Current Dialogue Line Text
            string currentText = _currentLineIndex == -1 ? _currentNPC.greeting : _currentNPC.dialogueLines[_currentLineIndex];
            GUI.Label(new Rect(boxX + 25, boxY + 45, boxW - 50, 65), $"<size=11>\"{currentText}\"</size>", new GUIStyle(GUI.skin.label) { wordWrap = true });

            // Action Buttons (Next & Farewell)
            int btnW = 120;
            int btnH = 30;
            int btnY = boxY + boxH - 42;

            bool isLastLine = _currentNPC.dialogueLines != null && _currentLineIndex >= _currentNPC.dialogueLines.Length - 1;
            string nextBtnText = isLastLine ? "✔ Finish" : "Next ▶";

            if (GUI.Button(new Rect(boxX + boxW - btnW - 20, btnY, btnW, btnH), nextBtnText))
            {
                AdvanceDialogue();
            }

            if (GUI.Button(new Rect(boxX + 20, btnY, 100, btnH), "Farewell ✕"))
            {
                EndDialogue();
            }
        }
    }
}
