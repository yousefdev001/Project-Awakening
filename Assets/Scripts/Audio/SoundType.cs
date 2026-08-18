namespace Awakening.Audio
{
    /// <summary>
    /// Classification of all sound effects played throughout Project Awakening.
    /// </summary>
    public enum SoundType
    {
        // Combat SFX
        AttackSlash,
        HeavyCleave,
        DodgeWhoosh,
        SkillCast,
        PlayerHurt,
        GroundSlam,

        // Monster SFX
        MonsterHurt,
        MonsterDeath,
        BossRoar,
        BossEnrage,
        BossDeath,

        // Items & World
        GoldChink,
        PotionDrink,
        ChestOpen,
        ItemEquip,
        InteractClick,

        // Progression & Fanfares
        LevelUpFanfare,
        QuestComplete,
        VictoryFanfare
    }
}
