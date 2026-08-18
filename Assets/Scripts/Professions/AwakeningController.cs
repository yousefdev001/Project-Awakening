using System;
using System.Collections;
using Awakening.Core;
using UnityEngine;

namespace Awakening.Professions
{
    public enum AwakeningStep
    {
        Idle,
        Analyzing,
        RankRevealed,
        ProfessionRevealed,
        Completed
    }

    /// <summary>
    /// Manages the cinematic sequence of the Awakening Rite.
    /// Orchestrates time transitions, energy analysis, rank reveal, and profession unveiling.
    /// </summary>
    public class AwakeningController : MonoBehaviour
    {
        public static AwakeningController Instance { get; private set; }

        [Header("Sequence Timings (Seconds)")]
        [SerializeField] private float _analysisDuration = 2.0f;
        [SerializeField] private float _rankRevealDuration = 1.5f;

        public AwakeningStep CurrentStep { get; private set; } = AwakeningStep.Idle;
        public bool IsAwakening => CurrentStep != AwakeningStep.Idle && CurrentStep != AwakeningStep.Completed;
        public ProfessionData RolledProfession { get; private set; }

        public event Action OnAwakeningStarted;
        public event Action<float> OnAnalysisProgress; // 0.0 to 1.0
        public event Action<ProfessionRank, Color> OnRankRevealed;
        public event Action<ProfessionData> OnProfessionRevealed;
        public event Action<ProfessionData> OnAwakeningCompleted;

        private Coroutine _sequenceCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void StartAwakeningSequence()
        {
            if (IsAwakening) return;

            if (_sequenceCoroutine != null)
            {
                StopCoroutine(_sequenceCoroutine);
            }

            _sequenceCoroutine = StartCoroutine(AwakeningSequenceRoutine());
        }

        private IEnumerator AwakeningSequenceRoutine()
        {
            CurrentStep = AwakeningStep.Analyzing;

            // 1. Enter Awakening GameState (stops player movement & unlocks cursor)
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Awakening);
            }

            OnAwakeningStarted?.Invoke();
            Debug.Log("<color=#00D4FF>[Awakening Rite]</color> The Magic Circle ignites... Analyzing magical potential...");

            // 2. Analysis Phase (Energy Build-up)
            float elapsed = 0f;
            while (elapsed < _analysisDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _analysisDuration);
                OnAnalysisProgress?.Invoke(progress);
                yield return null;
            }

            // 3. Roll Random Profession
            if (ProfessionRandomizer.Instance != null)
            {
                RolledProfession = ProfessionRandomizer.Instance.RollAndAssignProfession();
            }
            else
            {
                RolledProfession = ProfessionSystem.CreateSwordsmanPreset();
            }

            // 4. Rank Reveal
            CurrentStep = AwakeningStep.RankRevealed;
            Debug.Log($"<color=#FFD700>[Awakening Rite] ★ RANK REVEALED: {RolledProfession.rank} ★</color>");
            OnRankRevealed?.Invoke(RolledProfession.rank, RolledProfession.rankColor);

            yield return new WaitForSeconds(_rankRevealDuration);

            // 5. Profession Reveal
            CurrentStep = AwakeningStep.ProfessionRevealed;
            Debug.Log($"<color=#00FFAA>[Awakening Rite] ★ PROFESSION AWAKENED: {RolledProfession.professionName} ★</color>");
            OnProfessionRevealed?.Invoke(RolledProfession);
        }

        public void ConfirmAndFinishAwakening()
        {
            CurrentStep = AwakeningStep.Completed;

            // Refill player health and mana upon successful awakening
            if (Player.PlayerStats.Instance != null)
            {
                Player.PlayerStats.Instance.Heal(Player.PlayerStats.Instance.MaxHealth);
                Player.PlayerStats.Instance.RestoreMana(Player.PlayerStats.Instance.MaxMana);
            }

            // Return to normal gameplay
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }

            OnAwakeningCompleted?.Invoke(RolledProfession);
            Debug.Log("<color=#55FF55>[Awakening Rite]</color> Awakening ceremony concluded! Returning to gameplay.");

            CurrentStep = AwakeningStep.Idle;
        }
    }
}
