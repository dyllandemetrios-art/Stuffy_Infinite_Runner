using UnityEngine;

/// <summary>MonoBehaviour entry point that initializes and drives the game state machine.</summary>
public class GameStateController : MonoBehaviour
{
    private StateMachine _stateMachine;
    
    /// <summary>Creates the state machine and starts the game with the countdown state.</summary>
    private void Start()
    {
        _stateMachine = new StateMachine();
        var initialState = new CountdownState(_stateMachine);
        
        _stateMachine.ChangeState(initialState);
    }
    
    /// <summary>Forwards Unity's Update tick to the active state machine state.</summary>
    private void Update()
    {
        _stateMachine.Update();
    }
}