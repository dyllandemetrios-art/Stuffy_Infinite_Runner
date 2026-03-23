using TMPro;
using UnityEngine;

/// <summary>Displays the current score in real time and the final score on Game Over.</summary>
public class UIScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;         // Text displaying the score during the run.
    [SerializeField] private TMP_Text _finalScoreText;    // Text displaying the final score on the Game Over screen.

    private int _lastScore; // Last score value received, used to display it on Game Over.

    /// <summary>Subscribes to score and state events on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnScoreUpdated += HandleScoreUpdated;
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnScoreUpdated -= HandleScoreUpdated;
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Updates the in-run score text and caches the value for Game Over display.</summary>
    private void HandleScoreUpdated(int score)
    {
        _lastScore = score;

        if (_scoreText != null)
            _scoreText.text = "Distance : " + score + "m";
    }

    /// <summary>Shows the final score on Game Over screen when the state transitions.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameOverState)
            return;

        if (_finalScoreText != null)
            _finalScoreText.text = "Final Score : " + _lastScore + "m";
    }
}