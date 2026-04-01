using Components.SaveService;
using UnityEngine;

/// <summary>Reads skill levels from save data and applies their effects to game systems at run start.</summary>
public class SkillApplierController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LifeController _lifeController; // Reference to apply HP and drain modifications.

    // Base values matching Inspector defaults (GDD)
    private const float BaseMaxHP             = 100f; // Base max HP before upgrades.
    private const float BaseHealAmount        = 15f;  // Base heal amount per waste pickup.
    private const float BaseDamageMultiplier  = 1f;   // Base damage multiplier (no reduction).
    private const float BaseInvincibility     = 2f;   // Base invincibility duration in seconds.
    private const float BaseDrainPerSecond    = 3f;   // Base passive drain per second.

    /// <summary>Subscribes to state changes to apply skills at the start of each run.</summary>
    private void Awake()
    {
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Unsubscribes from state events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Applies all skill effects when transitioning into GameState.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState)
            return;

        var saveData = SaveService.Load();
        ApplySkills(saveData);
    }

    /// <summary>Reads each skill level and applies the corresponding stat modification.</summary>
    private void ApplySkills(SaveData saveData)
    {
        // Skill Health — max HP (GDD: 110/125/150)
        float maxHP = saveData.SkillHealth switch
        {
            1 => 110f,
            2 => 125f,
            3 => 150f,
            _ => BaseMaxHP
        };

        // Skill Recovery — heal amount (GDD: +10/+12/+15)
        float healAmount = saveData.SkillRecovery switch
        {
            1 => 10f,
            2 => 12f,
            3 => 15f,
            _ => BaseHealAmount
        };

        // Skill Armor — damage multiplier (GDD: -10%/-20%/-30%)
        float damageMultiplier = saveData.SkillArmor switch
        {
            1 => 0.9f,
            2 => 0.8f,
            3 => 0.7f,
            _ => BaseDamageMultiplier
        };

        // Skill Stabilizer — invincibility duration (GDD: 2/2.5/3s)
        float invincibilityDuration = saveData.SkillStabilizer switch
        {
            1 => 2f,
            2 => 2.5f,
            3 => 3f,
            _ => BaseInvincibility
        };

        // Skill Optimization — passive drain (GDD: -1.8/-1.5/-1.2/s)
        float drainPerSecond = saveData.SkillOptimization switch
        {
            1 => 1.8f,
            2 => 1.5f,
            3 => 1.2f,
            _ => BaseDrainPerSecond
        };

        // Apply all values to LifeController
        _lifeController.ApplySkills(maxHP, healAmount, damageMultiplier, invincibilityDuration, drainPerSecond);

        Debug.Log("[SkillApplier] Skills applied — HP: " + maxHP + " | Heal: " + healAmount + 
                  " | Armor: " + damageMultiplier + " | Invincibility: " + invincibilityDuration + 
                  " | Drain: " + drainPerSecond);
    }
}