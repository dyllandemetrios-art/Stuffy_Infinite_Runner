using UnityEngine;

/// <summary>Manages state transitions and forwards Update ticks to the currently active state.</summary>
public class StateMachine
{
    public State CurrentState; // The currently active state, accessible by external systems.
    
    /// <summary>Exits the current state, switches to the new one, and broadcasts the change via event.</summary>
    public void ChangeState(State newState)
    {
        Debug.Log("Changing state from: " + CurrentState?.GetType().Name + " to: " + newState.GetType().Name);
        
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        
        EventSystem.OnStateChanged?.Invoke(CurrentState);
    }
    
    /// <summary>Forwards the Unity Update tick to the active state each frame.</summary>
    public void Update()
    {
        CurrentState?.Update();
    }
}