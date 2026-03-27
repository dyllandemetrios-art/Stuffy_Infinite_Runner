using System.Collections;
using UnityEngine;

/// <summary>Controls enemy behavior: periodically throws projectiles toward the player.</summary>
public class EnnemyController : MonoBehaviour
{
    [SerializeField] private ProjectileController _projectile;        // Projectile prefab instantiated on each throw.
    [SerializeField] private float _throwingInterval = 1f;            // Time in seconds between each salve (GDD: 1 shot/2s).
    [SerializeField] private int _projectilesPerSalve = 1;            // Number of projectiles per salve (GDD: 1 to 3).
    [SerializeField] private float _delayBeforeFirstThrow = 0.2f;       // Delay in seconds before the first throw (GDD: 1s).
    [SerializeField] private float _delayBetweenProjectiles = 0.2f;   // Delay in seconds between each projectile in a salve.
    [SerializeField] private float _delayBeforeProjectile = 1f; // Delay in seconds before spawning the projectile after the animation starts.
    [SerializeField] private Animator _animator;                      // Animator component for throw animation.
    [SerializeField] private Transform _throwOrigin; // Spawn point of the projectile, placed on the enemy's hand.

    private Transform _playerTransform; // Cached player transform used to aim projectiles.

    /// <summary>Caches the player transform and starts the throw loop on spawn.</summary>
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ThrowLoop());
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

            // Wait before spawning to sync with the throw animation
            yield return new WaitForSeconds(_delayBeforeProjectile);

            var projectile = Instantiate(_projectile, _throwOrigin.position, Quaternion.identity);
            projectile.LaunchTowards(_playerTransform.GetChild(0).position);

            yield return new WaitForSeconds(_delayBetweenProjectiles);
        }
    }
}