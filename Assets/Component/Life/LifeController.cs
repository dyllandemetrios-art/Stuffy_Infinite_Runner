using System.Collections;
using UnityEngine;

/// <summary>Manages player HP (0-100), passive drain, collision damage, invincibility frames, and Game Over trigger.</summary>
public class LifeController : MonoBehaviour
{
    [Header("HP Parameters")]
    [SerializeField] private float _maxHP = 100f;                  // Maximum HP value (GDD: base 100).
    [SerializeField] private float _collisionDamage = 15f;         // HP lost on obstacle collision (GDD: -15 base).
    [SerializeField] private float _invincibilityDuration = 1.5f;  // Invincibility duration after a hit in seconds (GDD: 1.5s).

    [Header("Drain Parameters")]
    [SerializeField] private float _drainPerSecond = 2f;           // Passive HP lost per second (GDD: -2 HP/s).

    private float _currentHP;   // Current HP value during the run.
    private bool _isInvincible; // True during the invincibility window after a hit.
    private bool _inGameState;  // True only while the game is in an active run.

    /// <summary>Subscribes to state and collision events on initialization.</summary>
    private void Start()
    {
        _currentHP = _maxHP;

        EventSystem.OnPlayerHPUpdated?.Invoke(_currentHP);
        EventSystem.OnPlayerCollision += HandlePlayerCollision;
        EventSystem.OnStateChanged += HandleStateChanged;
        EventSystem.OnPlayerHealed += HandlePlayerHealed;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnPlayerCollision -= HandlePlayerCollision;
        EventSystem.OnStateChanged -= HandleStateChanged;
        EventSystem.OnPlayerHealed -= HandlePlayerHealed;
    }

    /// <summary>Applies passive HP drain each frame during an active run.</summary>
    private void Update()
    {
        if (!_inGameState)
            return;

        _currentHP -= _drainPerSecond * Time.deltaTime;
        _currentHP = Mathf.Max(_currentHP, 0f);

        EventSystem.OnPlayerHPUpdated?.Invoke(_currentHP);

        if (_currentHP <= 0f)
        {
            _inGameState = false;
            EventSystem.OnPlayerLifeUpdated?.Invoke(0);
        }
    }

    /// <summary>Activates or deactivates the drain loop based on the current game state.</summary>
    private void HandleStateChanged(State newState)
    {
        _inGameState = newState is GameState;

        // Reset HP at the start of each new run
        if (_inGameState)
        {
            _currentHP = _maxHP;
            EventSystem.OnPlayerHPUpdated?.Invoke(_currentHP);
        }
    }

    /// <summary>Applies collision damage, triggers invincibility, and fires Game Over if HP reaches zero.</summary>
    private void HandlePlayerCollision()
    {
        if (_isInvincible)
            return;

        _currentHP -= _collisionDamage;
        _currentHP = Mathf.Max(_currentHP, 0f);

        EventSystem.OnPlayerHPUpdated?.Invoke(_currentHP);

        if (_currentHP <= 0f)
        {
            _inGameState = false;
            EventSystem.OnPlayerLifeUpdated?.Invoke(0);
            return;
        }

        EventSystem.OnPlayerLifeUpdated?.Invoke((int)_currentHP);
        EventSystem.OnPlayerInvincibilityStarted?.Invoke(_invincibilityDuration);
        StartCoroutine(InvincibilityCoroutine());
    }

    /// <summary>Blocks incoming damage for the invincibility duration after a hit.</summary>
    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityDuration);
        _isInvincible = false;
    }
    
    /// <summary>Adds heal amount to current HP without exceeding the maximum.</summary>
    private void HandlePlayerHealed(float healAmount)
    {
        _currentHP = Mathf.Min(_currentHP + healAmount, _maxHP);
        EventSystem.OnPlayerHPUpdated?.Invoke(_currentHP);
    }
}