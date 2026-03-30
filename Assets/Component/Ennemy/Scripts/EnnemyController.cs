using System.Collections;
using UnityEngine;

/// <summary>Controls enemy behavior: periodically throws projectiles toward the player, stops when passed.</summary>
public class EnnemyController : MonoBehaviour
{
    [SerializeField] private ProjectileController _projectile;
    [SerializeField] private float _throwingInterval = 1f;
    [SerializeField] private int _projectilesPerSalve = 1;
    [SerializeField] private float _delayBeforeFirstThrow = 0.2f;
    [SerializeField] private float _delayBetweenProjectiles = 0.2f;
    [SerializeField] private float _delayBeforeProjectile = 1f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _throwOrigin;

    private Transform _playerTransform;
    private Coroutine _throwLoopCoroutine; // Cached coroutine reference for stopping when enemy is passed.

    /// <summary>Caches the player transform and starts the throw loop on spawn.</summary>
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _throwLoopCoroutine = StartCoroutine(ThrowLoop());
    }

    /// <summary>Stops the throw loop when the enemy has passed the player on the Z axis.</summary>
    private void Update()
    {
        if (transform.position.z < 0 && _throwLoopCoroutine != null)
        {
            StopCoroutine(_throwLoopCoroutine);
            _throwLoopCoroutine = null;
        }
    }

    /// <summary>Waits for the initial delay then throws a salve at each interval indefinitely.</summary>
    private IEnumerator ThrowLoop()
    {
        yield return new WaitForSeconds(_delayBeforeFirstThrow);

        while (true)
        {
            yield return StartCoroutine(ThrowSalve());
            yield return new WaitForSeconds(_throwingInterval);
        }
    }

    /// <summary>Triggers the throw animation and spawns each projectile of the salve with a small delay between them.</summary>
    private IEnumerator ThrowSalve()
    {
        for (int i = 0; i < _projectilesPerSalve; i++)
        {
            _animator.SetTrigger("Throwing");

            yield return new WaitForSeconds(_delayBeforeProjectile);

            var projectile = Instantiate(_projectile, _throwOrigin.position, Quaternion.identity);
            projectile.LaunchTowards(_playerTransform.GetChild(0).position);

            yield return new WaitForSeconds(_delayBetweenProjectiles);
        }
    }
}