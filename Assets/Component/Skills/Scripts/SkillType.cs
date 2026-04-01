/// <summary>Defines all available skill types in the progression system (GDD pages 27-28).</summary>
public enum SkillType
{
    Health,       // Structural reinforcement : increases max HP.
    Recovery,     // Recovery system : increases heal amount from waste pickups.
    Armor,        // Armor : reduces collision damage.
    Stabilizer,   // Stabilizer : increases invincibility duration after a hit.
    Optimization  // Optimization : reduces passive HP drain per second.
}