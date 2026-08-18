using UnityEngine;

namespace Awakening.Monsters
{
    /// <summary>
    /// ScriptableObject defining all stats and combat attributes for a Monster.
    /// Data-Driven: Adding new monsters requires zero C# script modifications.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMonsterData", menuName = "Awakening/Monsters/Monster Data")]
    public class MonsterData : ScriptableObject
    {
        [Header("Identity")]
        public string monsterID = "MON_SLIME";
        public string monsterName = "Green Slime";
        public MonsterRank rank = MonsterRank.Normal;
        public int level = 1;
        public Color themeColor = new Color(0.2f, 0.9f, 0.3f);
        public Vector3 modelScale = new Vector3(1f, 0.8f, 1f);

        [Header("Vitals & Defense")]
        public float maxHealth = 60.0f;
        public float defense = 2.0f;

        [Header("Locomotion")]
        public float patrolSpeed = 2.0f;
        public float chaseSpeed = 4.0f;

        [Header("Combat Attributes")]
        public float attackPower = 8.0f;
        public float attackRange = 1.8f;
        public float attackCooldown = 1.8f;
        public float detectionRadius = 8.0f;

        [Header("Rewards")]
        public float xpReward = 35.0f;
        public int minGold = 2;
        public int maxGold = 8;

        #region MVP Monster Presets Factory
        public static MonsterData CreateSlimePreset()
        {
            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterID = "MON_SLIME";
            data.monsterName = "Green Slime";
            data.rank = MonsterRank.Normal;
            data.level = 1;
            data.themeColor = new Color(0.25f, 0.95f, 0.35f);
            data.modelScale = new Vector3(1.1f, 0.75f, 1.1f);
            data.maxHealth = 60f;
            data.defense = 2f;
            data.patrolSpeed = 1.8f;
            data.chaseSpeed = 3.5f;
            data.attackPower = 8f;
            data.attackRange = 1.5f;
            data.attackCooldown = 1.8f;
            data.detectionRadius = 7.0f;
            data.xpReward = 35f;
            return data;
        }

        public static MonsterData CreateWolfPreset()
        {
            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterID = "MON_WOLF";
            data.monsterName = "Wild Forest Wolf";
            data.rank = MonsterRank.Normal;
            data.level = 3;
            data.themeColor = new Color(0.45f, 0.5f, 0.6f);
            data.modelScale = new Vector3(0.9f, 0.9f, 1.4f);
            data.maxHealth = 110f;
            data.defense = 5f;
            data.patrolSpeed = 3.0f;
            data.chaseSpeed = 6.0f;
            data.attackPower = 16f;
            data.attackRange = 2.0f;
            data.attackCooldown = 1.4f;
            data.detectionRadius = 11.0f;
            data.xpReward = 75f;
            return data;
        }

        public static MonsterData CreateGoblinPreset()
        {
            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterID = "MON_GOBLIN";
            data.monsterName = "Goblin Warrior";
            data.rank = MonsterRank.Elite;
            data.level = 5;
            data.themeColor = new Color(0.85f, 0.35f, 0.15f);
            data.modelScale = new Vector3(0.95f, 1.1f, 0.95f);
            data.maxHealth = 180f;
            data.defense = 8f;
            data.patrolSpeed = 2.5f;
            data.chaseSpeed = 5.0f;
            data.attackPower = 24f;
            data.attackRange = 2.2f;
            data.attackCooldown = 1.5f;
            data.detectionRadius = 10.0f;
            data.xpReward = 130f;
            return data;
        }
        #endregion
    }
}
