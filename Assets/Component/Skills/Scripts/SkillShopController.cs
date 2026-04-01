using Components.SaveService;
using TMPro;
using UnityEngine;

/// <summary>Manages skill purchases, validates component cost, persists progression to JSON, and displays component count.</summary>
public class SkillShopController : MonoBehaviour
{
    [SerializeField] private TMP_Text _componentCountText; // Displays available components in the shop.

    // Skill costs per level (GDD pages 27-28)
    private static readonly int[] CostHealth       = { 10, 20, 40 }; // Structural reinforcement costs.
    private static readonly int[] CostRecovery     = { 10, 20, 40 }; // Recovery system costs.
    private static readonly int[] CostArmor        = { 15, 30, 50 }; // Armor costs.
    private static readonly int[] CostStabilizer   = { 10, 20, 40 }; // Stabilizer costs.
    private static readonly int[] CostOptimization = { 15, 30, 50 }; // Optimization costs.

    private SaveData _saveData; // Loaded save data, modified on purchase and written to disk.

    /// <summary>Loads save data and broadcasts current state to all UI buttons on initialization.</summary>
    private void Start()
    {
        _saveData = SaveService.Load();
        BroadcastCurrentState();
    }

    /// <summary>Fires skill state events so all UI buttons can refresh their display on scene load.</summary>
    private void BroadcastCurrentState()
    {
        _componentCountText.text = "Components : " + _saveData.Components;

        EventSystem.OnSkillStateUpdated?.Invoke(SkillType.Health,       _saveData.SkillHealth,       _saveData.Components);
        EventSystem.OnSkillStateUpdated?.Invoke(SkillType.Recovery,     _saveData.SkillRecovery,     _saveData.Components);
        EventSystem.OnSkillStateUpdated?.Invoke(SkillType.Armor,        _saveData.SkillArmor,        _saveData.Components);
        EventSystem.OnSkillStateUpdated?.Invoke(SkillType.Stabilizer,   _saveData.SkillStabilizer,   _saveData.Components);
        EventSystem.OnSkillStateUpdated?.Invoke(SkillType.Optimization, _saveData.SkillOptimization, _saveData.Components);
    }

    /// <summary>Attempts to purchase the next level of the given skill, deducting components if affordable.</summary>
    public void TryBuySkill(SkillType skillType)
    {
        int currentLevel = GetSkillLevel(skillType);
        int[] costs = GetSkillCosts(skillType);

        // Already maxed out
        if (currentLevel >= 3)
        {
            Debug.Log("[SkillShop] Skill already at max level : " + skillType);
            return;
        }

        int cost = costs[currentLevel];

        // Not enough components
        if (_saveData.Components < cost)
        {
            Debug.Log("[SkillShop] Not enough components. Need " + cost + ", have " + _saveData.Components);
            return;
        }

        // Deduct cost and increment skill level
        _saveData.Components -= cost;
        SetSkillLevel(skillType, currentLevel + 1);

        SaveService.Save(_saveData);
        BroadcastCurrentState();

        Debug.Log("[SkillShop] Purchased " + skillType + " level " + (currentLevel + 1));
    }

    /// <summary>Returns the current level of the given skill from save data.</summary>
    private int GetSkillLevel(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Health       => _saveData.SkillHealth,
            SkillType.Recovery     => _saveData.SkillRecovery,
            SkillType.Armor        => _saveData.SkillArmor,
            SkillType.Stabilizer   => _saveData.SkillStabilizer,
            SkillType.Optimization => _saveData.SkillOptimization,
            _ => 0
        };
    }

    /// <summary>Sets the skill level in save data for the given skill type.</summary>
    private void SetSkillLevel(SkillType skillType, int level)
    {
        switch (skillType)
        {
            case SkillType.Health:       _saveData.SkillHealth       = level; break;
            case SkillType.Recovery:     _saveData.SkillRecovery     = level; break;
            case SkillType.Armor:        _saveData.SkillArmor        = level; break;
            case SkillType.Stabilizer:   _saveData.SkillStabilizer   = level; break;
            case SkillType.Optimization: _saveData.SkillOptimization = level; break;
        }
    }

    /// <summary>Returns the cost array for the given skill type.</summary>
    private int[] GetSkillCosts(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Health       => CostHealth,
            SkillType.Recovery     => CostRecovery,
            SkillType.Armor        => CostArmor,
            SkillType.Stabilizer   => CostStabilizer,
            SkillType.Optimization => CostOptimization,
            _ => CostHealth
        };
    }
}