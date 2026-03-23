using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Static service centralizing scene loading logic for the game.</summary>
public static class SceneLoaderService
{
    /// <summary>Loads the gameplay scene and its UI additively on top.</summary>
    public static void LoadGame()
    {
        Debug.Log("Loading Game...");
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
        SceneManager.LoadScene("LevelUI", LoadSceneMode.Additive);
        Debug.Log("Game loaded");
    }
    
    /// <summary>Loads the main menu scene, unloading all currently active scenes.</summary>
    public static void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Debug.Log("Main Menu loaded");
    }
}