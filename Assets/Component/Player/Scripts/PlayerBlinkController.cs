using System.Collections;
using UnityEngine;

/// <summary>Handles player blinking during invincibility, with exponentially increasing frequency as it ends.</summary>
public class PlayerBlinkController : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;       // All renderers to toggle for the blink effect.
    [SerializeField] private float _minBlinkInterval = 0.05f; // Fastest blink interval at end of invincibility.
    [SerializeField] private float _maxBlinkInterval = 0.2f;  // Slowest blink interval at start of invincibility.

    private Coroutine _blinkCoroutine;

    /// <summary>Subscribes to invincibility event on initialization.</summary>
    private void Awake()
    {
        EventSystem.OnPlayerInvincibilityStarted += HandleInvincibilityStarted;
    }

    /// <summary>Unsubscribes from invincibility event to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnPlayerInvincibilityStarted -= HandleInvincibilityStarted;
    }

    /// <summary>Starts the blink coroutine when invincibility begins.</summary>
    private void HandleInvincibilityStarted(float duration)
    {
        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine(duration));
    }

    /// <summary>Blinks the player renderers with exponentially increasing frequency over the invincibility duration.</summary>
    private IEnumerator BlinkCoroutine(float duration)
    {
        float elapsed = 0f;
        bool visible = true;
        float blinkTimer = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blinkTimer += Time.deltaTime;

            float ratio = elapsed / duration;
            float interval = Mathf.Lerp(_maxBlinkInterval, _minBlinkInterval, ratio * ratio);

            if (blinkTimer >= interval)
            {
                visible = !visible;
                SetRenderersVisible(visible);
                blinkTimer = 0f;
            }

            yield return null;
        }

        // Ensure player is visible when invincibility ends
        SetRenderersVisible(true);
        _blinkCoroutine = null;
    }

    /// <summary>Toggles visibility on all renderers simultaneously.</summary>
    private void SetRenderersVisible(bool visible)
    {
        foreach (var r in _renderers)
            r.enabled = visible;
    }
}