using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>Manages chunk spawning, despawning, translation, and progressive speed increases during a run.</summary>
public class ObstacleController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip("Translation speed of chunks in m/s")] private float _translationSpeed = 3f;
    [SerializeField] private int _activeChunkCount = 5;   // Number of chunks kept active ahead of the player.
    [SerializeField] private int _behindChunkCount = 1;   // Number of passed chunks to keep before destroying them.
    [SerializeField] private float _stopDelayOnDamage = 0.2f; // Duration in seconds the world stops moving after the player takes damage.
    
    [Header("Components")]
    [SerializeField] private ChunkController[] _chunksPool; // Pool of chunk prefabs randomly selected during spawn.

    [Header("Speed Up")] 
    [SerializeField, Tooltip("Interval in seconds between each speed increases")] private float _speedUpInterval = 15f;
    [SerializeField, Tooltip("Speed increase applied on each interval")] private float _speedUpIncrease = 1.5f;
    
    private readonly List<ChunkController> _instancedChunks = new(); // Currently active chunk instances in the scene.
    private float _baseTranslationSpeed; // Reference speed used to restore movement after a damage stop.
    
    private float _stopDelayTimer; // Accumulates time elapsed since the movement stop was triggered.
    private bool _stopped;         // True while the world is paused following a player collision.
    private bool _inGameState;     // Tracks whether the game is currently in an active run.

    private GameState _gameState;    // Cached reference to read the run timer for speed up logic.
    private int _lastSpeedUpTime;    // Last timer value at which a speed increase was applied, prevents duplicate triggers.
    
    /// <summary>Subscribes to state change events on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Stores the base speed and spawns the initial set of chunks at game start.</summary>
    private void Start()
    {
        _baseTranslationSpeed = _translationSpeed;
        _translationSpeed = 0;
        
        AddBaseChunk();
    }
    
    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnPlayerLifeUpdated -= HandlePlayerLifeUpdated;
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Drives movement, speed up logic, and chunk lifecycle each frame during GameState.</summary>
    private void Update()
    {
        if (!_inGameState)
        {
            return;
        }
        
        ResetMovementAfterDelay();
        TranslateChunks();
        UpdateChunks();
    }

    /// <summary>Restores movement speed after the damage stop delay has elapsed.</summary>
    private void ResetMovementAfterDelay()
    {
        if (!_stopped) 
            return;
        
        _stopDelayTimer += Time.deltaTime;
        if (_stopDelayTimer >= _stopDelayOnDamage)
        {
            _stopped = false;
            _translationSpeed = _baseTranslationSpeed;
            _stopDelayTimer = 0f;
        }
    }
    
    /// <summary>Moves all active chunks backward and applies a speed increase at each interval.</summary>
    private void TranslateChunks()
    {
        var gameTimer = _gameState.Timer;
        if (gameTimer != 0 && gameTimer % _speedUpInterval == 0 && gameTimer != _lastSpeedUpTime)
        {
            _translationSpeed += _speedUpIncrease;
            _baseTranslationSpeed = _translationSpeed;
            _lastSpeedUpTime = gameTimer;
            EventSystem.OnSpeedUpdated?.Invoke(_translationSpeed);
        }
        
        foreach (var chunk in _instancedChunks)
        {
            chunk.transform.Translate(Vector3.back * (_translationSpeed * Time.deltaTime));
        }
    }

    /// <summary>Destroys excess chunks behind the player and spawns new ones ahead to maintain active count.</summary>
    private void UpdateChunks()
    {
        List<ChunkController> behindChunks = new();

        foreach (var chunk in _instancedChunks)
        {
            if (chunk.IsBehindPlayer())
            {
                behindChunks.Add(chunk);
            }
        }

        // Delete potential chunks behind player.
        if (behindChunks.Count > _behindChunkCount)
        {
            int chunkToDeleteCount = behindChunks.Count - _behindChunkCount;

            for (int i = 0; i < chunkToDeleteCount; i++)
            {
                var chunkToDelete = behindChunks[i];
                _instancedChunks.Remove(chunkToDelete);
                
                Destroy(chunkToDelete.gameObject);
            }
        }
        
        // Add potential new chunks.
        int missingChunkCount = _activeChunkCount - _instancedChunks.Count;
        for (int i = 0; i < missingChunkCount; i++)
        {
            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancedChunks.Add(chunk);
        }
    }

    /// <summary>Spawns the initial set of chunks chained from the controller's position.</summary>
    private void AddBaseChunk()
    {
        for (int i = 0; i < _activeChunkCount; i++)
        {
            if (i == 0)
            {
                var baseChunk = AddChunk(transform.position);
                _instancedChunks.Add(baseChunk);
                continue;
            }

            var chunk = AddChunk(LastActiveChunk().EndAnchor);
            _instancedChunks.Add(chunk);
        }
    }

    /// <summary>Instantiates a random chunk from the pool at the given world position.</summary>
    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0)
        {
            Debug.LogError("No chunks in pool");
            return null;
        }
        
        var index = Random.Range(0, _chunksPool.Length);
        ChunkController chunk = Instantiate(_chunksPool[index], position, Quaternion.identity);
        
        return chunk;
    }
    
    /// <summary>Returns the last chunk in the active list, used to chain new chunk spawn positions.</summary>
    private ChunkController LastActiveChunk()
    {
        return _instancedChunks[_instancedChunks.Count - 1];
    }
    
    /// <summary>Stops world movement on player hit, or zeroes speed permanently on player death.</summary>
    private void HandlePlayerLifeUpdated(int playerLifeCount)
    {
        if (playerLifeCount > 0)
        {
            _stopped = true;
        }
        
        _translationSpeed = 0;
    }
    
    /// <summary>Activates or deactivates chunk management and movement based on the current game state.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState gameState)
        {
            EventSystem.OnPlayerLifeUpdated -= HandlePlayerLifeUpdated;
            _inGameState = false;
            return;
        }

        _gameState = gameState;
        _translationSpeed = _baseTranslationSpeed;
        EventSystem.OnSpeedUpdated?.Invoke(_translationSpeed);
        EventSystem.OnPlayerLifeUpdated += HandlePlayerLifeUpdated;
        _inGameState = true;
    }
}