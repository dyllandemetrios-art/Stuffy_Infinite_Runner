using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays skill info and handles purchase button for a single skill type.</summary>
public class UISkillButtonController : MonoBehaviour
{
    [SerializeField] private SkillType _skillType;          // Skill this button represents, set in Inspector.
    [SerializeField] private TMP_Text _levelText;           // Displays current level (e.g. "Level 2 / 3").
    [SerializeField] private TMP_Text _costText;            // Displays next level cost or "MAX" if maxed.
    [SerializeField] private TMP_Text _descriptionText;     // Displays skill effect description.
    [SerializeField] private Button _buyButton;             // Button to trigger purchase.
    [SerializeField] private SkillShopController _shop;     // Reference to the shop controller.

    // Costs per level for display purposes only (logic is in SkillShopController)
    private static readonly int[][] AllCosts =
    {
        new[] { 10, 20, 40 }, // Health
        new[] { 10, 20, 40 }, // Recovery
        new[] { 15, 30, 50 }, // Armor
        new[] { 10, 20, 40 }, // Stabilizer
        new[] { 15, 30, 50 }, // Optimization
    };

    // Descriptions per skill and level
    private static readonly string[][] AllDescriptions =
    {
        new[] { "Max HP : 110", "Max HP : 125", "Max HP : 150" },                          // Health
        new[] { "Heal : +10 HP", "Heal : +12 HP", "Heal : +15 HP" },                      // Recovery
        new[] { "Armor : -10%", "Armor : -20%", "Armor : -30%" },                         // Armor
        new[] { "Invincibility : 2s", "Invincibility : 2.5s", "Invincibility : 3s" },     // Stabilizer
        new[] { "Drain : -1.8/s", "Drain : -1.5/s", "Drain : -1.2/s" },                  // Optimization
    };

    /// <summary>Subscribes to skill state updates when the button becomes active.</summary>
    private void OnEnable()
    {
        EventSystem.OnSkillStateUpdated += HandleSkillStateUpdated;
    }

    /// <summary>Unsubscribes from skill state events when the button becomes inactive.</summary>
    private void OnDisable()
    {
        EventSystem.OnSkillStateUpdated -= HandleSkillStateUpdated;
    }

    /// <summary>Refreshes the button display when this skill's state changes.</summary>
    private void HandleSkillStateUpdated(SkillType skillType, int currentLevel, int availableComponents)
    {
        // Only update if this event concerns this button's skill
        if (skillType != _skillType)
            return;

        _levelText.text = "Level " + currentLevel + " / 3";

        // Update description based on current level
        if (currentLevel == 0)
            _descriptionText.text = "Next : " + AllDescriptions[(int)_skillType][0];
        else if (currentLevel >= 3)
            _descriptionText.text = AllDescriptions[(int)_skillType][2];
        else
            _descriptionText.text = "Next : " + AllDescriptions[(int)_skillType][currentLevel];

        if (currentLevel >= 3)
        {
            _costText.text = "MAX";
            _buyButton.interactable = false;
            return;
        }

        int cost = AllCosts[(int)_skillType][currentLevel];
        _costText.text = "Cost : " + cost;
        _buyButton.interactable = availableComponents >= cost;
    }

    /// <summary>Called by the buy button OnClick — triggers skill purchase in the shop controller.</summary>
    public void OnBuyButtonClicked()
    {
        _shop.TryBuySkill(_skillType);
    }
}