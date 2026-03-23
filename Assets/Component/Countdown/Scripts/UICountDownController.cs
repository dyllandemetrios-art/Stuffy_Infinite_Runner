using System;
using TMPro;
using UnityEngine;

/// <summary>Displays the countdown window and timer value, visible only during CountdownState.</summary>
public class UICountDownController : MonoBehaviour
{
    [SerializeField] private GameObject _window;        // Root window shown only during the countdown.
    [SerializeField] private TMP_Text _countdownText;   // Text element displaying the remaining countdown value.
    
    private bool _inCountdown;               // Tracks whether the game is currently in CountdownState.
    private CountdownState _countdownState;  // Cached reference to read the timer value each frame.
    
    /// <summary>Hides the window and subscribes to state change events on initialization.</summary>
    private void Awake()
    {
        _window.SetActive(false);
        EventSystem.OnStateChanged += HandleStateChanged;
    }
    
    /// <summary>Unsubscribes from state events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Shows or hides the countdown window and caches the CountdownState reference on transitions.</summary>
    private void HandleStateChanged(State state)
    {
        if (state is not CountdownState countdownState)
        {
            _inCountdown = false;
            _window.SetActive(false);
            return;
        }

        _window.SetActive(true);
        _countdownState = countdownState;
        _inCountdown = true;
    }

    /// <summary>Updates the countdown text each frame while in CountdownState.</summary>
    private void Update()
    {
        if (!_inCountdown)
        {
            return;
        }

        _countdownText.text = _countdownState.Timer.ToString("0");
    }
}