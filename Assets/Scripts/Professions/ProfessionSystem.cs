using System;
using Awakening.Player;
using UnityEngine;

namespace Awakening.Professions
{
    /// <summary>
    /// Manages the player's current profession, rank, and integrates attribute modifiers with PlayerStats.
    /// </summary>
    public class ProfessionSystem : MonoBehaviour
    {
        public static ProfessionSystem Instance { get; private set; }

        [Header("Current Profession")]
        [SerializeField] private ProfessionData _currentProfession;

        public ProfessionData CurrentProfession => _currentProfession;
        public bool HasProfession => _currentProfession != null;
        public bool HasAwakened => HasProfession;
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

            AcquirePlayerStats();
        }

        private void Start()
        {
            AcquirePlayerStats();

            if (_currentProfession != null)
            {
                ApplyProfessionModifiers(_currentProfession);
            }
        }

        private void AcquirePlayerStats()
        {
            if (_playerStats == null)
            {
                _playerStats = GetComponent<PlayerStats>() 
                    ?? PlayerStats.Instance 
                    ?? FindFirstObjectByType<PlayerStats>();
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
            AcquirePlayerStats();

            if (_playerStats == null)
            {
                Debug.LogError("[ProfessionSystem] Cannot apply modifiers: PlayerStats component not found in the scene!");
                return;
            }

            // Update Primary Attributes
            _playerStats.BonusVitality = profession.bonusVitality;
            _playerStats.BonusIntelligence = profession.bonusIntelligence;

            // Update Direct Pools & Combat Stats
            _playerStats.BonusMaxHealth = profession.bonusMaxHealth;
            _playerStats.BonusMaxMana = profession.bonusMaxMana;
            _playerStats.BonusAttack = profession.bonusAttack;
            _playerStats.BonusDefense = profession.bonusDefense;
            _playerStats.BonusSpeed = profession.bonusSpeed;

            // Recalculate runtime stats and refill HP/MP for immediate visual reward
            _playerStats.RecalculateStats(true);
        }

        public void RemoveProfession()
        {
            _currentProfession = null;
            AcquirePlayerStats();

            if (_playerStats != null)
            {
                _playerStats.BonusVitality = 0f;
                _playerStats.BonusIntelligence = 0f;
                _playerStats.BonusMaxHealth = 0f;
                _playerStats.BonusMaxMana = 0f;
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
            data.description = "A versatile warrior skilled in blade arts and physical combat with high Vitality and Defense.";
            data.rankColor = new Color(0.3f, 0.85f, 0.4f); // Emerald Green
            data.bonusVitality = 8f;
            data.bonusIntelligence = 1f;
            data.bonusMaxHealth = 30f;
            data.bonusMaxMana = 10f;
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
            data.description = "A swift marksman excelling in rapid movement, ranged archery, and balanced energy.";
            data.rankColor = new Color(0.2f, 0.6f, 1.0f); // Sapphire Blue
            data.bonusVitality = 4f;
            data.bonusIntelligence = 6f;
            data.bonusMaxHealth = 20f;
            data.bonusMaxMana = 40f;
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
            data.description = "An elite spell-warrior wielding devastating elemental magic with massive Intelligence and Mana.";
            data.rankColor = new Color(0.85f, 0.35f, 1.0f); // Mystic Purple
            data.bonusVitality = 6f;
            data.bonusIntelligence = 18f;
            data.bonusMaxHealth = 40f;
            data.bonusMaxMana = 120f;
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
