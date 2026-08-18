using System;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Professions
{
    /// <summary>
    /// Manages the player's current profession, rank, and integrates stat modifiers with PlayerStats.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    public class ProfessionSystem : MonoBehaviour
    {
        public static ProfessionSystem Instance { get; private set; }

        [Header("Current Profession")]
        [SerializeField] private ProfessionData _currentProfession;

        public ProfessionData CurrentProfession => _currentProfession;
        public bool HasProfession => _currentProfession != null;
        public string ProfessionName => HasProfession ? _currentProfession.professionName : "Unawakened";
        public ProfessionRank CurrentRank => HasProfession ? _currentProfession.rank : ProfessionRank.RankC;

        public event Action<ProfessionData> OnProfessionAssigned;

        private PlayerStats _playerStats;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _playerStats = GetComponent<PlayerStats>();
        }

        private void Start()
        {
            if (_currentProfession != null)
            {
                ApplyProfessionModifiers(_currentProfession);
            }
        }

        public void AssignProfession(ProfessionData profession)
        {
            if (profession == null) return;

            _currentProfession = profession;
            ApplyProfessionModifiers(_currentProfession);

            Debug.Log($"<color=#00FFAA>[ProfessionSystem]</color> Player Awakened as: <b>[{_currentProfession.rank}] {_currentProfession.professionName}</b>!");

            OnProfessionAssigned?.Invoke(_currentProfession);
        }

        private void ApplyProfessionModifiers(ProfessionData profession)
        {
            if (_playerStats == null) return;

            // Update PlayerStats bonus fields
            _playerStats.BonusMaxHealth = profession.bonusMaxHealth;
            _playerStats.BonusAttack = profession.bonusAttack;
            _playerStats.BonusDefense = profession.bonusDefense;
            _playerStats.BonusSpeed = profession.bonusSpeed;

            // Recalculate runtime stats
            _playerStats.RecalculateStats(false);
        }

        public void RemoveProfession()
        {
            _currentProfession = null;
            if (_playerStats != null)
            {
                _playerStats.BonusMaxHealth = 0f;
                _playerStats.BonusAttack = 0f;
                _playerStats.BonusDefense = 0f;
                _playerStats.BonusSpeed = 0f;
                _playerStats.RecalculateStats(false);
            }
            OnProfessionAssigned?.Invoke(null);
        }

        #region MVP Presets Factory
        public static ProfessionData CreateSwordsmanPreset()
        {
            var data = ScriptableObject.CreateInstance<ProfessionData>();
            data.professionID = "PROF_SWORDSMAN";
            data.professionName = "Swordsman";
            data.rank = ProfessionRank.RankC;
            data.description = "A versatile warrior skilled in blade arts and physical combat.";
            data.rankColor = new Color(0.3f, 0.85f, 0.4f); // Emerald Green
            data.bonusMaxHealth = 40f;
            data.bonusAttack = 12f;
            data.bonusDefense = 10f;
            data.bonusSpeed = 0f;
            data.weaponAffinity = "Sword";
            data.skillName = "Heavy Slash";
            data.skillDescription = "Delivers a heavy physical slash dealing 180% damage.";
            return data;
        }

        public static ProfessionData CreateHunterPreset()
        {
            var data = ScriptableObject.CreateInstance<ProfessionData>();
            data.professionID = "PROF_HUNTER";
            data.professionName = "Hunter";
            data.rank = ProfessionRank.RankB;
            data.description = "A swift marksman excelling in rapid movement and ranged archery.";
            data.rankColor = new Color(0.2f, 0.6f, 1.0f); // Sapphire Blue
            data.bonusMaxHealth = 25f;
            data.bonusAttack = 22f;
            data.bonusDefense = 4f;
            data.bonusSpeed = 1.5f; // Fast movement bonus
            data.weaponAffinity = "Bow";
            data.skillName = "Piercing Arrow";
            data.skillDescription = "Shoots a high-velocity piercing arrow through targets.";
            return data;
        }

        public static ProfessionData CreateBattleMagePreset()
        {
            var data = ScriptableObject.CreateInstance<ProfessionData>();
            data.professionID = "PROF_BATTLE_MAGE";
            data.professionName = "Battle Mage";
            data.rank = ProfessionRank.RankA;
            data.description = "An elite spell-warrior who wields devastating elemental magic.";
            data.rankColor = new Color(0.85f, 0.35f, 1.0f); // Mystic Purple
            data.bonusMaxHealth = 60f;
            data.bonusAttack = 38f;
            data.bonusDefense = 12f;
            data.bonusSpeed = 0.5f;
            data.weaponAffinity = "Staff";
            data.skillName = "Arcane Burst";
            data.skillDescription = "Unleashes an explosive burst of magical energy.";
            return data;
        }
        #endregion
    }
}
