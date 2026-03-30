using UnityEngine;

/// <summary>Detects player collisions using an overlap sphere that shrinks when sliding down.</summary>
public class PlayerCollisionController : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private Vector3 _sphereCenter;       // Center offset of the normal collision sphere.
    [SerializeField] private float _sphereRadius;         // Radius of the normal collision sphere.
    [SerializeField] private Vector3 _shrinkSphereCenter; // Center offset of the reduced sphere used when sliding down.
    [SerializeField] private float _shrinkSphereRadius;   // Radius of the reduced sphere used when sliding down.
    
    private bool _isHit; // Prevents firing multiple collision events while overlapping the same collider.
    
    private Vector3 _currentSphereCenter; // Active sphere center, swapped on slide down.
    private float _currentSphereRadius;   // Active sphere radius, swapped on slide down.

    private Vector3 PlayerSpherePosition => transform.position + _currentSphereCenter; // World position of the active sphere.
    
    /// <summary>Initializes the active collider to normal size and subscribes to slide down events.</summary>
    private void Start()
    {
        _currentSphereCenter = _sphereCenter;
        _currentSphereRadius = _sphereRadius;
        
        EventSystem.OnPlayerSlideDown += ShrinkCollider;
    }
    
    /// <summary>Unsubscribes from events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnPlayerSlideDown -= ShrinkCollider;
    }

    /// <summary>Checks for overlapping colliders each frame, fires collision or pickup event based on tag.</summary>
    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(PlayerSpherePosition, _currentSphereRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Pickup"))
            {
                hit.GetComponent<PickupController>()?.Collect();
                continue;
            }

            if (hit.CompareTag("Component"))
            {
                hit.GetComponent<ComponentPickupController>()?.Collect();
                continue;
            }

            if (!_isHit)
            {
                EventSystem.OnPlayerCollision?.Invoke();
                _isHit = true;
            }

            break;
        }

        bool hasObstacleContact = false;
        foreach (var hit in hitColliders)
        {
            if (!hit.CompareTag("Pickup") && !hit.CompareTag("Component"))
            {
                hasObstacleContact = true;
                break;
            }
        }

        if (!hasObstacleContact)
            _isHit = false;
    }

    /// <summary>Switches between normal and shrunk sphere dimensions based on slide down state.</summary>
    private void ShrinkCollider(bool isSlidingDown)
    {
        if (isSlidingDown)
        {
            _currentSphereCenter = _shrinkSphereCenter;
            _currentSphereRadius = _shrinkSphereRadius;
        }
        else
        {
            _currentSphereCenter = _sphereCenter;
            _currentSphereRadius = _sphereRadius;
        }
    }

    /// <summary>Draws all three spheres in the editor for collision debugging.</summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _sphereCenter, _sphereRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _shrinkSphereCenter, _shrinkSphereRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(PlayerSpherePosition, _currentSphereRadius);
    }
}