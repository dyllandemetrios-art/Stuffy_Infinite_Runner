using UnityEngine;

/// <summary>Tracks distance travelled as score using chunk speed and broadcasts updates each frame.</summary>
public class ScoreController : MonoBehaviour
{
    private float _distance;   // Total distance travelled in the current run in metres.
    private float _speed;      // Current chunk translation speed received from ObstacleController.
    private bool _inGameState; // True only while the game is in an active run.

    /// <summary>Subscribes to state and speed events on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnStateChanged += HandleStateChanged;
        EventSystem.OnSpeedUpdated += HandleSpeedUpdated;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
        EventSystem.OnSpeedUpdated -= HandleSpeedUpdated;
    }

    /// <summary>Increments distance each frame based on current speed and broadcasts the updated score.</summary>
    private void Update()
    {
        if (!_inGameState)
            return;

        _distance += _speed * Time.deltaTime;
        EventSystem.OnScoreUpdated?.Invoke(Mathf.FloorToInt(_distance));
    }

    /// <summary>Resets distance on run start and deactivates tracking on other states.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState)
        {
            _inGameState = false;
            return;
        }

        _distance = 0f;
        _inGameState = true;
    }

    /// <summary>Keeps local speed in sync with the chunk translation speed from ObstacleController.</summary>
    private void HandleSpeedUpdated(float newSpeed)
    {
        _speed = newSpeed;
    }
}