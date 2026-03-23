using UnityEngine;

/// <summary>State that runs a countdown timer before transitioning to GameState.</summary>
public class CountdownState : State
{
    private float _initialTime = 3f; // Duration of the countdown in seconds.
    private float _timer;
    
    public float Timer => _timer; // Remaining countdown time, readable by external systems (e.g. UI).

    public CountdownState(StateMachine stateMachine) : base(stateMachine) { }

    /// <summary>Resets the timer to its initial value on state entry.</summary>
    public override void Enter()
    {
        _timer = _initialTime;
    }

    /// <summary>Decrements the timer and transitions to GameState when it reaches zero.</summary>
    public override void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0)
        {
            return;
        }
        
        // Go to game state
        var gameState = new GameState(StateMachine);
        StateMachine.ChangeState(gameState);
    }

    public override void Exit()
    {
        
    }
}