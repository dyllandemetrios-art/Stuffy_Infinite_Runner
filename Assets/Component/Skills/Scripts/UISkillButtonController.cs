using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays skill info and handles purchase button for a single skill type.</summary>
public class UISkillButtonController : MonoBehaviour
{
    [SerializeField] private SkillType _skillType;          // Skill this button represents, set in Inspector.
    [SerializeField] private TMP_Text _levelText;           // Displays current level (e.g. "Level 2 / 3").
    [SerializeField] private TMP_Text _costText;            // Displays next level cost or "MAX" if maxed.
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

    /// <summary>Subscribes to skill state updates on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnSkillStateUpdated += HandleSkillStateUpdated;
    }

    /// <summary>Unsubscribes from skill state events to prevent memory leaks.</summary>
    private void OnDestroy()
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