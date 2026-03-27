using UnityEngine;

/// <summary>Moves a projectile toward a target position and destroys it on arrival or after its lifetime expires.</summary>
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float _speed = 10;                    // Movement speed of the projectile in m/s (GDD: 10 m/s).
    [SerializeField] private float _projectileLifetime = 2f; // Time in seconds before the projectile auto-destroys (GDD: 3s).
    
    private Vector3 _targetPosition; // World position the projectile moves toward.
    private float _timer;            // Accumulates time elapsed since the projectile was launched.

    /// <summary>Sets the target position the projectile will move toward.</summary>
    public void LaunchTowards(Vector3 position)
    {
        _targetPosition = position;
    }

    /// <summary>Moves the projectile toward the target each frame and destroys it on arrival or timeout.</summary>
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

        if (transform.position == _targetPosition)
        {
            Destroy(gameObject);
        }
        
        _timer += Time.deltaTime;
        if (_timer > _projectileLifetime)
        {
            Destroy(gameObject);
        }
    }
}