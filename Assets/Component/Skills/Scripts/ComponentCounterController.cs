using Components.SaveService;
using UnityEngine;

/// <summary>Tracks components collected during a run, persists total to JSON, and broadcasts updates for the HUD.</summary>
public class ComponentCounterController : MonoBehaviour
{
    private int _componentsThisRun; // Components collected during the current run only.
    private SaveData _saveData;     // Loaded save data, updated on component collection.

    /// <summary>Loads save data and subscribes to component and state events on initialization.</summary>
    private void Start()
    {
        _saveData = SaveService.Load();
        EventSystem.OnComponentCollected += HandleComponentCollected;
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnComponentCollected -= HandleComponentCollected;
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Resets run counter on game start.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState)
            return;

        _componentsThisRun = 0;
        EventSystem.OnComponentCountUpdated?.Invoke(_saveData.Components);
    }

    /// <summary>Increments run and total counters, saves to disk, and broadcasts the updated total.</summary>
    private void HandleComponentCollected(int amount)
    {
        _componentsThisRun += amount;
        _saveData.Components += amount;

        SaveService.Save(_saveData);
        EventSystem.OnComponentCountUpdated?.Invoke(_saveData.Components);
    }
}