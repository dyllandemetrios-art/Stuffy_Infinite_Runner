using System;
using Components.SaveService;
using TMPro;
using UnityEngine;

/// <summary>Drives the main menu UI, displaying saved stats and handling game start and quit actions.</summary>
public class UIMainMenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text _runCountText; // Displays the total number of attempts from save data.
    [SerializeField] private TMP_Text _bestTimeText; // Displays the best run time or a fallback message.
    private SaveData _saveData; // Cached save data loaded on menu start.
    
    /// <summary>Loads save data and populates the run count and best time display on menu open.</summary>
    private void Start()
    {
        _saveData = SaveService.Load();
        _runCountText.text = "Attempts: " + _saveData.RunCount;

        if (_saveData.BestTime == 0)
        {
            _bestTimeText.text = "No Best Time";
        }
        else
        {
            var timeSpan = new TimeSpan(0, 0, _saveData.BestTime);
            _bestTimeText.text = "Best Time: " + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }
    }

    /// <summary>Increments the run count, saves it to disk, then loads the game scene.</summary>
    public void StartGame()
    {
        _saveData.RunCount++;
        SaveService.Save(_saveData);
        
        SceneLoaderService.LoadGame();
    }
    
    /// <summary>Quits the application, or stops play mode when running inside the Unity Editor.</summary>
    public void QuitGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}