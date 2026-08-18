using System;
using System.Collections;
using Awakening.Core;
using Awakening.Player;
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

            // 3. Roll Random Profession (Generate without applying until Embrace)
            if (ProfessionRandomizer.Instance != null)
            {
                RolledProfession = ProfessionRandomizer.Instance.RollAndAssignProfession();
            }
            else
            {
                RolledProfession = ProfessionSystem.CreateSwordsmanPreset();
                if (ProfessionSystem.Instance != null)
                {
                    ProfessionSystem.Instance.AssignProfession(RolledProfession);
                }
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

            // 1. Explicitly ensure Profession and Stats are applied and synced
            if (RolledProfession != null)
            {
                if (ProfessionSystem.Instance != null)
                {
                    ProfessionSystem.Instance.AssignProfession(RolledProfession);
                }
                else if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.BonusVitality = RolledProfession.bonusVitality;
                    PlayerStats.Instance.BonusIntelligence = RolledProfession.bonusIntelligence;
                    PlayerStats.Instance.BonusMaxHealth = RolledProfession.bonusMaxHealth;
                    PlayerStats.Instance.BonusMaxMana = RolledProfession.bonusMaxMana;
                    PlayerStats.Instance.BonusAttack = RolledProfession.bonusAttack;
                    PlayerStats.Instance.BonusDefense = RolledProfession.bonusDefense;
                    PlayerStats.Instance.BonusSpeed = RolledProfession.bonusSpeed;
                    PlayerStats.Instance.RecalculateStats(true);
                }
            }

            // 2. Refill player health and mana upon successful awakening
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.Heal(PlayerStats.Instance.MaxHealth);
                PlayerStats.Instance.RestoreMana(PlayerStats.Instance.MaxMana);
            }

            // 3. Return to normal gameplay
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetState(GameState.Gameplay);
            }

            OnAwakeningCompleted?.Invoke(RolledProfession);
            Debug.Log("<color=#55FF55>[Awakening Rite]</color> Awakening ceremony concluded! Returning to gameplay with boosted stats.");

            CurrentStep = AwakeningStep.Idle;
        }
    }
}
