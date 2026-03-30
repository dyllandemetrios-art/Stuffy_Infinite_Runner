namespace Components.SaveService
{
    /// <summary>Serializable data container holding persistent player stats and skill levels across sessions.</summary>
    [System.Serializable]
    public class SaveData
    {
        public int RunCount;   // Total number of runs completed by the player.
        public int BestTime;   // Best time achieved across all runs, in seconds.
        public int Components; // Total electronic components collected across all runs.

        // =============================================
        // SKILL LEVELS — 0 = locked, 1/2/3 = upgraded
        // (GDD pages 27-28)
        // =============================================
        public int SkillHealth;         // Structural reinforcement : max HP 110/125/150.
        public int SkillRecovery;       // Recovery system : waste heal +10/+12/+15.
        public int SkillArmor;          // Armor : damage reduction -10%/-20%/-30%.
        public int SkillStabilizer;     // Stabilizer : invincibility 2/2.5/3s.
        public int SkillOptimization;   // Optimization : passive drain -1.8/-1.5/-1.2/s.
    }
}