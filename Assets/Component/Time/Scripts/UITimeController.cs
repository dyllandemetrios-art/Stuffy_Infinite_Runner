using System;
using TMPro;
using UnityEngine;

/// <summary>Displays the elapsed run time in mm:ss format, visible only during GameState.</summary>
public class UITimeController : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText; // UI text element used to display the formatted timer.
    
    private GameState _gameState;  // Reference to the active GameState to read the timer value.
    private bool _inGameState;     // Tracks whether the game is currently in an active run.
    
    /// <summary>Subscribes to state changes and hides the timer on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnStateChanged += HandleStateChanged;
        _timeText.gameObject.SetActive(false);
    }

    /// <summary>Unsubscribes from state events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Shows or hides the timer and caches the GameState reference on state transitions.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState gameState)
        {
            _inGameState = false;
            _timeText.gameObject.SetActive(false);
            return;
        }
        
        _gameState = gameState;
        _inGameState = true;
        _timeText.gameObject.SetActive(true);
    }

    /// <summary>Updates the timer display each frame while in GameState.</summary>
    private void Update()
    {
        if (!_inGameState)
        {
            return;
        }

        var timeSpan = new TimeSpan(0, 0, _gameState.Timer);
        
        // Display time on the following format mm:ss
        _timeText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
    }
}