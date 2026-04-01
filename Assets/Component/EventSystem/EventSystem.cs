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
    public static Action<float> OnPlayerInvincibilityStarted; // Fired when invincibility starts, passes duration in seconds.
    public static Action<int> OnComponentCollected; // Fired when the player collects an electronic component.
    public static Action<int> OnComponentCountUpdated; // Fired when the total component count changes.
    public static Action OnPlayerHit; // Fired only on actual obstacle or projectile collision, not passive drain.
    public static Action<SkillType, int, int> OnSkillStateUpdated; // Fired when a skill level changes — passes skill type, current level, and available components.
    
    public static Action<State> OnStateChanged;       // Fired when the state machine transitions to a new state.
}