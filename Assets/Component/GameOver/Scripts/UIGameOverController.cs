using UnityEngine;

/// <summary>Shows the game over screen when GameOverState is active and handles main menu navigation.</summary>
public class UIGameOverController : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverScreen; // Root panel shown only when the game is over.
    
    /// <summary>Hides the game over screen and subscribes to state change events on initialization.</summary>
    private void Awake()
    {
        _gameOverScreen.SetActive(false);
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Unsubscribes from state events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }
    
    /// <summary>Toggles the game over screen visibility based on whether the current state is GameOverState.</summary>
    private void HandleStateChanged(State newState)
    {
        _gameOverScreen.SetActive(newState is GameOverState);
    }
    
    /// <summary>Loads the main menu scene, called by the UI restart or menu button.</summary>
    public void LoadMainMenu()
    {
        SceneLoaderService.LoadMainMenu();
    }
}