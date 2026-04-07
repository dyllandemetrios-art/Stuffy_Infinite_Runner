using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Handles player movement: lane sliding, jumping, and sliding down, with interruptible actions and ground detection.</summary>
public class PlayerMovementController : MonoBehaviour
{
    [Header("Jump parameters")]
    [SerializeField, Tooltip("Duration of jump in seconds")] private float _jumpDuration = 1.1f;
    [SerializeField] private float _jumpHeight = 2.5f;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private AnimationCurve _fallCurve;

    [Header("Slide parameters")]
    [SerializeField] private float _slideDuration = 0.12f;
    [SerializeField] private Transform[] _slideTarget;

    [Header("Slide Down parameters")]
    [SerializeField] private float _slideDownDuration = 0.8f;

    [Header("Speed Scaling")]
    [SerializeField] private float _baseTranslationSpeed = 7f; // Initial chunk speed, must match ObstacleController.
    [SerializeField] private float _minSlideDuration = 0.08f;  // Minimum slide duration at max speed.
    [SerializeField] private float _minJumpDuration = 0.6f;    // Minimum jump duration at max speed.

    [Header("Ground Detection")]
    [SerializeField] private LayerMask _groundLayer;           // Layer mask used for downward raycast ground detection.
    [SerializeField] private float _groundRaycastDistance = 10f; // Maximum raycast distance to detect ground below player.

    [Header("Components")]
    [SerializeField] private Animator _animator;

    [Header("Debug")]
    [SerializeField] private int _currentLaneIndex = 1;
    [SerializeField] private bool _isSliding;
    [SerializeField] private bool _isSlidingDown;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _locked;

    private float _groundY;                  // Y position of the ground, updated each frame via raycast.
    private Coroutine _slideCoroutine;       // Cached lane slide coroutine for interruption.
    private Coroutine _jumpCoroutine;        // Cached jump coroutine for interruption.
    private Coroutine _slideDownCoroutine;   // Cached slide down coroutine for interruption.

    /// <summary>Subscribes to state and speed events, locks movement until game starts.</summary>
    private void Awake()
    {
        EventSystem.OnStateChanged += HandleStateChanged;
        EventSystem.OnSpeedUpdated += HandleSpeedUpdated;
        _locked = true;
    }

    /// <summary>Initializes ground Y from current position as fallback before first raycast.</summary>
    private void Start()
    {
        _groundY = transform.position.y;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnPlayerLifeUpdated -= HandlePlayerLifeUpdated;
        EventSystem.OnStateChanged -= HandleStateChanged;
        EventSystem.OnSpeedUpdated -= HandleSpeedUpdated;
    }

    /// <summary>Locks or unlocks movement based on the current game state.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is not GameState)
        {
            _locked = true;
            StopAllCoroutines();
            EventSystem.OnPlayerLifeUpdated -= HandlePlayerLifeUpdated;
            return;
        }

        _animator.SetTrigger("Running");
        EventSystem.OnPlayerLifeUpdated += HandlePlayerLifeUpdated;
        _locked = false;
    }

    /// <summary>Triggers death animation on player death, damage feedback handled by blink only.</summary>
    private void HandlePlayerLifeUpdated(int playerLife)
    {
        if (playerLife > 0)
            return;

        // Snap to ground before stopping coroutines to avoid mid-air freeze
        transform.position = new Vector3(transform.position.x, _groundY, transform.position.z);
        _isJumping = false;
        _animator.SetBool("IsJumping", false);

        StopAllCoroutines();
        _animator.SetTrigger("Dead");
        _locked = true;
    }

    /// <summary>Scales slide and jump durations down as chunk translation speed increases.</summary>
    private void HandleSpeedUpdated(float newSpeed)
    {
        float ratio = _baseTranslationSpeed / newSpeed;
        _slideDuration = Mathf.Max(_minSlideDuration, 0.12f * ratio);
        _jumpDuration = Mathf.Max(_minJumpDuration, 1.1f * ratio);
    }

    /// <summary>Updates ground Y each frame via raycast, then dispatches input to movement handlers.</summary>
    public void Update()
    {
        if (_locked)
            return;

        UpdateGroundY();
        HandleJump();
        HandleSlide();
        HandleSlideDown();
    }

    /// <summary>Casts a ray downward to update ground Y position each frame, only when not jumping.</summary>
    private void UpdateGroundY()
    {
        if (_isJumping)
            return;

        // Cast from slightly above current position to avoid missing ground when snapped to it
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _groundRaycastDistance, _groundLayer))
        {
            _groundY = hit.point.y;
        }
    }

    /// <summary>Starts jump coroutine on press, interrupting slide down if active.</summary>
    private void HandleJump()
    {
        bool jumpPressed = Keyboard.current.upArrowKey.wasPressedThisFrame
                        || Keyboard.current.spaceKey.wasPressedThisFrame;

        if (!jumpPressed)
            return;

        if (_isSlidingDown)
            InterruptSlideDown();

        if (_isJumping)
            return;

        _jumpCoroutine = StartCoroutine(JumpCoroutine());
    }

    /// <summary>Moves player left or right between lanes on key press, interrupting slide down if active.</summary>
    private void HandleSlide()
    {
        bool leftPressed = Keyboard.current.leftArrowKey.wasPressedThisFrame
                        || Keyboard.current.qKey.wasPressedThisFrame;

        bool rightPressed = Keyboard.current.rightArrowKey.wasPressedThisFrame
                         || Keyboard.current.dKey.wasPressedThisFrame;

        if (leftPressed)
        {
            if (_isSlidingDown) InterruptSlideDown();

            if (_isSliding)
            {
                StopCoroutine(_slideCoroutine);
                _isSliding = false;
            }

            if (_currentLaneIndex == 0)
                return;

            _currentLaneIndex--;
            _slideCoroutine = StartCoroutine(SlideCoroutine(_slideTarget[_currentLaneIndex]));
        }

        if (rightPressed)
        {
            if (_isSlidingDown) InterruptSlideDown();

            if (_isSliding)
            {
                StopCoroutine(_slideCoroutine);
                _isSliding = false;
            }

            if (_currentLaneIndex == _slideTarget.Length - 1)
                return;

            _currentLaneIndex++;
            _slideCoroutine = StartCoroutine(SlideCoroutine(_slideTarget[_currentLaneIndex]));
        }
    }

    /// <summary>Starts or restarts slide down coroutine on key press, interrupting jump if active.</summary>
    private void HandleSlideDown()
    {
        bool downPressed = Keyboard.current.downArrowKey.wasPressedThisFrame
                        || Keyboard.current.fKey.wasPressedThisFrame;

        if (!downPressed)
            return;

        if (_isJumping)
            InterruptJump();

        if (_isSlidingDown)
            InterruptSlideDown();

        _slideDownCoroutine = StartCoroutine(SlideDownCoroutine());
    }

    /// <summary>Stops the jump coroutine and snaps the player back to ground Y.</summary>
    private void InterruptJump()
    {
        if (_jumpCoroutine != null)
            StopCoroutine(_jumpCoroutine);

        _isJumping = false;
        _animator.SetBool("IsJumping", false);
        transform.position = new Vector3(transform.position.x, _groundY, transform.position.z);
    }

    /// <summary>Stops the slide down coroutine and restores the player's normal collider state.</summary>
    private void InterruptSlideDown()
    {
        if (_slideDownCoroutine != null)
            StopCoroutine(_slideDownCoroutine);

        _isSlidingDown = false;
        _animator.SetBool("IsSlidingDown", false);
        EventSystem.OnPlayerSlideDown?.Invoke(false);
    }

    /// <summary>Moves player vertically using animation curves relative to the detected ground Y position.</summary>
    private IEnumerator JumpCoroutine()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SoundEffect.Jump);
        
        _isJumping = true;
        _animator.SetBool("IsJumping", true);

        // Capture ground Y before jumping to guarantee correct landing position
        float jumpGroundY = _groundY;

        float jumpTimer = 0f;
        float halfJumpDuration = _jumpDuration / 2f;

        // Jump phase
        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            var normalizedTime = jumpTimer / halfJumpDuration;
            var targetHeight = jumpGroundY + _jumpCurve.Evaluate(normalizedTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            yield return null;
        }

        // Fall phase
        _animator.SetTrigger("Falling");
        jumpTimer = 0f;

        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            var normalizedTime = jumpTimer / halfJumpDuration;
            var targetHeight = jumpGroundY + _fallCurve.Evaluate(normalizedTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            yield return null;
        }

        // Always snap back to the exact ground Y captured before jump
        transform.position = new Vector3(transform.position.x, jumpGroundY, transform.position.z);
        _groundY = jumpGroundY;
        _isJumping = false;
        _animator.SetBool("IsJumping", false);
    }

    /// <summary>Lerps player horizontally to the target lane from a captured start position.</summary>
    private IEnumerator SlideCoroutine(Transform target)
    {
        _isSliding = true;
        var slideTimer = 0f;
        var startPosition = transform.position;

        while (slideTimer < _slideDuration)
        {
            slideTimer += Time.deltaTime;
            var normalizedTime = Mathf.Clamp01(slideTimer / _slideDuration);
            var targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
            yield return null;
        }

        // Snap to exact lane position to avoid floating point drift
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        _isSliding = false;
    }

    /// <summary>Triggers slide down state and notifies other systems for a fixed duration.</summary>
    private IEnumerator SlideDownCoroutine()
    {
        _isSlidingDown = true;
        _animator.SetBool("IsSlidingDown", true);
        EventSystem.OnPlayerSlideDown?.Invoke(true);

        var slideTimer = 0f;
        while (slideTimer <= _slideDownDuration)
        {
            slideTimer += Time.deltaTime;
            yield return null;
        }

        _isSlidingDown = false;
        _animator.SetBool("IsSlidingDown", false);
        EventSystem.OnPlayerSlideDown?.Invoke(false);
    }
}