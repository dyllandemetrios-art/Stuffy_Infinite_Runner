using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>Manages chunk spawning, despawning, translation, and progressive speed increases during a run.</summary>
public class ObstacleController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip("Translation speed of chunks in m/s")] private float _translationSpeed = 9f;
    [SerializeField] private int _activeChunkCount = 5;
    [SerializeField] private int _behindChunkCount = 1;

    [Header("Components")]
    [SerializeField] private ChunkController[] _chunksPool;

    [Header("Speed Up")]
    [SerializeField] private float _maxTranslationSpeed = 16f;
    [SerializeField] private float _speedUpInterval = 10f;
    [SerializeField] private float _speedUpPercentage = 0.12f;

    private readonly List<ChunkController> _instancedChunks = new();
    private float _baseTranslationSpeed;
    private bool _inGameState;

    private GameState _gameState;
    private int _lastSpeedUpTime;

    /// <summary>Subscribes to state change events and initializes base speed on initialization.</summary>
    private void Awake()
    {
        _baseTranslationSpeed = _translationSpeed;
        _translationSpeed = 0;
        EventSystem.OnStateChanged += HandleStateChanged;
    }

    /// <summary>Spawns the initial set of chunks at game start.</summary>
    private void Start()
    {
        AddBaseChunk();
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>Drives movement, speed up logic, and chunk lifecycle each frame during GameState.</summary>
    private void Update()
    {
        if (!_inGameState)
            return;

        TranslateChunks();
        UpdateChunks();
    }

    /// <summary>Moves all active chunks backward and applies a speed increase at each interval.</summary>
    private void TranslateChunks()
    {
        var gameTimer = _gameState.Timer;
        if (gameTimer != 0 && gameTimer % _speedUpInterval == 0 && gameTimer != _lastSpeedUpTime)
        {
            if (_baseTranslationSpeed > 0)
            {
                _translationSpeed += _baseTranslationSpeed * _speedUpPercentage;
                _translationSpeed = Mathf.Min(_translationSpeed, _maxTranslationSpeed);
                _baseTranslationSpeed = _translationSpeed;
                _lastSpeedUpTime = gameTimer;

                EventSystem.OnSpeedUpdated?.Invoke(_translationSpeed);
                Debug.Log("[ObstacleController] Speed: " + _translationSpeed);
            }
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
                behindChunks.Add(chunk);
        }

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

    /// <summary>Instantiates a weighted random chunk from the pool at the given world position.</summary>
    private ChunkController AddChunk(Vector3 position)
    {
        if (_chunksPool.Length == 0)
        {
            Debug.LogError("No chunks in pool");
            return null;
        }

        var chunk = Instantiate(GetWeightedRandomChunk(), position, Quaternion.identity);
        return chunk;
    }

    /// <summary>Selects a chunk prefab from the pool using weighted random.</summary>
    private ChunkController GetWeightedRandomChunk()
    {
        int totalWeight = 0;
        foreach (var chunk in _chunksPool)
            totalWeight += chunk.SpawnWeight;

        int random = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var chunk in _chunksPool)
        {
            cumulative += chunk.SpawnWeight;
            if (random < cumulative)
                return chunk;
        }

        return _chunksPool[0];
    }

    /// <summary>Returns the last chunk in the active list.</summary>
    private ChunkController LastActiveChunk()
    {
        return _instancedChunks[_instancedChunks.Count - 1];
    }

    /// <summary>Activates or deactivates chunk management based on the current game state.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState gameState)
        {
            _inGameState = false;
            _translationSpeed = 0;
            return;
        }

        _gameState = gameState;
        _translationSpeed = _baseTranslationSpeed;
        EventSystem.OnSpeedUpdated?.Invoke(_translationSpeed);
        _inGameState = true;
    }
}