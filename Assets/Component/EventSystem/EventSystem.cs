using System;

/// <summary>Central static event bus decoupling communication between all game systems.</summary>
public static class EventSystem
{
    public static Action<bool> OnPlayerSlideDown;     // Fired when the player starts or stops sliding down.
    public static Action OnPlayerCollision;            // Fired when the player's sphere overlaps a collider.
    public static Action<int> OnPlayerLifeUpdated;    // Fired when the player's life value changes.
    public static Action<float> OnPlayerHPUpdated;    // Fired when the player's HP value changes (0-100).
    public static Action<float> OnPlayerHealed;       // Fired when the player collects a healing pickup.
    public static Action<float> OnSpeedUpdated;       // Fired when the chunk translation speed changes.
    public static Action<int> OnScoreUpdated;         // Fired each frame with the current score in metres.

    
    public static Action<State> OnStateChanged;       // Fired when the state machine transitions to a new state.
}