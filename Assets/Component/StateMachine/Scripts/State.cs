/// <summary>Abstract base class for all game states, providing access to the parent state machine.</summary>
public abstract class State
{
    protected readonly StateMachine StateMachine; // Reference to the state machine managing this state.
    
    /// <summary>Stores the state machine reference for use by derived states.</summary>
    protected State(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    
    public abstract void Enter();  // Called once when the state becomes active.
    public abstract void Update(); // Called every frame while the state is active.
    public abstract void Exit();   // Called once when the state is replaced by another.
}