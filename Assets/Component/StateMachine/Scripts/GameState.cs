using Components.SaveService;
using UnityEngine;

/// <summary>State representing an active run, tracking elapsed time and handling player death.</summary>
public class GameState : State
{
    public GameState(StateMachine stateMachine) : base(stateMachine) { }
    
    public int Timer => Mathf.RoundToInt(_timer); // Elapsed run time in whole seconds, readable by external systems (e.g. UI).

    private float _timer;
    
    /// <summary>Subscribes to life events and resets the timer on run start.</summary>
    public override void Enter()
    {
        EventSystem.OnPlayerLifeUpdated += HandlePlayerLifeUpdated;
        _timer = 0;
    }

    /// <summary>Increments the run timer each frame.</summary>
    public override void Update()
    {
        _timer += Time.deltaTime;
    }

    public override void Exit()
    {
        var saveData = SaveService.Load();
        if (saveData.BestTime < Timer)
        {
            saveData.BestTime = Timer;
            SaveService.Save(saveData);
        }

        // Correctly unsubscribe on exit
        EventSystem.OnPlayerLifeUpdated -= HandlePlayerLifeUpdated;
    }
    
    /// <summary>Transitions to GameOverState when player life reaches zero.</summary>
    private void HandlePlayerLifeUpdated(int playerLife)
    {
        if (playerLife > 0)
        {
            return;
        }
        
        var gameOverState = new GameOverState(StateMachine);
        StateMachine.ChangeState(gameOverState);
    }
}