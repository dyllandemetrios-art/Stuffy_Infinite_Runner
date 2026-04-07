using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Updates HP bar fill, text, and color feedback based on current player HP.</summary>
public class UILifeView : MonoBehaviour
{
    [SerializeField] private TMP_Text _hpText;  // Text displaying the current HP value.
    [SerializeField] private Image _hpBar;      // Image filled proportionally to current HP (Image Type: Filled).

    [Header("Color Feedback")]
    [SerializeField] private Color _colorHigh = Color.green;    // Bar color when HP is above danger threshold.
    [SerializeField] private Color _colorLow = Color.red;       // Bar color when HP is at or below danger threshold.
    [SerializeField] private float _dangerThreshold = 0.3f;     // Fill ratio below which the bar turns red (GDD: ≤30 HP).

    private float _maxHP = 100f;

    private void Awake()
    {
        EventSystem.OnPlayerHPUpdated += HandlePlayerHPUpdated;
        EventSystem.OnMaxHPUpdated += HandleMaxHPUpdated;
    }

    private void OnDestroy()
    {
        EventSystem.OnPlayerHPUpdated -= HandlePlayerHPUpdated;
        EventSystem.OnMaxHPUpdated -= HandleMaxHPUpdated;
    }

    /// <summary>Updates the max HP reference when skills are applied at run start.</summary>
    private void HandleMaxHPUpdated(float newMaxHP)
    {
        _maxHP = newMaxHP;
    }

    /// <summary>Refreshes HP text, bar fill, and color when a new HP value is received.</summary>
    private void HandlePlayerHPUpdated(float newHP)
    {
        float fillRatio = newHP / _maxHP;

        _hpText.text = Mathf.CeilToInt(newHP) + " / " + (int)_maxHP;

        if (_hpBar != null)
        {
            _hpBar.fillAmount = fillRatio;
            _hpBar.color = fillRatio <= _dangerThreshold ? _colorLow : _colorHigh;
        }
    }
}