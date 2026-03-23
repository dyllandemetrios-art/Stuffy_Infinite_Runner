using UnityEngine;

/// <summary>State representing the end of a run, triggered when the player dies.</summary>
public class GameOverState : State
{
    public GameOverState(StateMachine stateMachine) : base(stateMachine) { }

    /// <summary>Called when entering the game over state, intended for save and UI logic.</summary>
    public override void Enter()
    {
        
    }

    /// <summary>Called each frame during game over, intended for input handling (e.g. restart).</summary>
    public override void Update()
    {
    }

    /// <summary>Called when leaving the game over state, intended for cleanup logic.</summary>
    public override void Exit()
    {
        
    }
}