using TMPro;
using UnityEngine;

/// <summary>Displays the total component count in the HUD, updated on each collection.</summary>
public class UIComponentCounterController : MonoBehaviour
{
    [SerializeField] private TMP_Text _componentText; // Text element displaying the component count.

    /// <summary>Subscribes to component count updates on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnComponentCountUpdated += HandleComponentCountUpdated;
    }

    /// <summary>Unsubscribes from events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnComponentCountUpdated -= HandleComponentCountUpdated;
    }

    /// <summary>Refreshes the component count text when a new value is received.</summary>
    private void HandleComponentCountUpdated(int count)
    {
        _componentText.text = "Cards : " + count;
    }
}