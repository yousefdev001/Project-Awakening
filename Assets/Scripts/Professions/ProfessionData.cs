using UnityEngine;

namespace Awakening.Professions
{
    /// <summary>
    /// ScriptableObject defining the blueprint and modifiers of a Profession.
    /// Data-Driven: Adding new professions requires only creating a new asset.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProfession", menuName = "Awakening/Professions/Profession Data")]
    public class ProfessionData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier string")]
        public string professionID = "PROF_SWORDSMAN";

        [Tooltip("Display name of the profession")]
        public string professionName = "Swordsman";

        [Tooltip("Profession Rank tier")]
        public ProfessionRank rank = ProfessionRank.RankC;

        [TextArea(2, 4)]
        public string description = "A disciplined melee warrior proficient in swords and frontline combat.";

        [Tooltip("Theme color associated with the rank")]
        public Color rankColor = new Color(0.2f, 0.8f, 0.3f); // Green for C

        [Header("Stat Modifiers")]
        [Tooltip("Flat bonus Max Health granted to the player")]
        public float bonusMaxHealth = 30f;

        [Tooltip("Flat bonus Attack power granted to the player")]
        public float bonusAttack = 10f;

        [Tooltip("Flat bonus Defense granted to the player")]
        public float bonusDefense = 8f;

        [Tooltip("Flat bonus Speed added to the player")]
        public float bonusSpeed = 0f;

        [Header("Affinity & Skills")]
        [Tooltip("Favored weapon type (Sword, Bow, Staff)")]
        public string weaponAffinity = "Sword";

        [Tooltip("Name of the active primary skill")]
        public string skillName = "Heavy Slash";

        [TextArea(1, 3)]
        public string skillDescription = "Delivers a powerful sweeping strike dealing 180% Physical Damage.";
    }
}
