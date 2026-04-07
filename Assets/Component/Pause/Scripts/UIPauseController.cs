using System;
using UnityEngine.InputSystem;
using UnityEngine;

/// <summary>Manages the pause panel visibility and game time scale during gameplay.</summary>
public class UIPauseController : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel; // Root panel shown only when the game is paused.
    [SerializeField] private GameObject _firstSelectedPause; // First button focused on menu open.
    
    private bool _isPaused; // Tracks whether the game is currently paused.

    /// <summary>Hides the pause panel and subscribes to state changes on initialization.</summary>
    private void Awake()
    {
        _pausePanel.SetActive(false);
        EventSystem.OnStateChanged += HandleStateChanged;
    }
    
    /// <summary>Unsubscribes from state events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Listens for Escape key each frame to toggle pause during GameState only.</summary>
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    /// <summary>Forces unpause and hides panel when leaving GameState.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState)
        {
            Unpause();
        }
    }

    /// <summary>Toggles between paused and unpaused states.</summary>
    public void TogglePause()
    {
        if (_isPaused)
            Unpause();
        else
            Pause();
    }

    /// <summary>Pauses the game by stopping time and showing the pause panel.</summary>
    public void Pause()
    {
        _isPaused = true;
        _pausePanel.SetActive(true);
        AudioManager.Instance?.PauseMusic();
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(_firstSelectedPause);
        Time.timeScale = 0f; // Set AFTER panel and selection are ready
    }

    /// <summary>Resumes the game by restoring time and hiding the pause panel.</summary>
    public void Unpause()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
        AudioManager.Instance.ResumeMusic();
    }

    /// <summary>Returns to main menu and ensures time scale is restored before scene load.</summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoaderService.LoadMainMenu();
    }
}