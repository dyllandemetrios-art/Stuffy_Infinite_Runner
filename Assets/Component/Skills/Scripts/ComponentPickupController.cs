using UnityEngine;

/// <summary>Awards one electronic component on collection and destroys itself when picked up.</summary>
public class ComponentPickupController : MonoBehaviour
{
    [SerializeField] private int _componentAmount = 1; // Number of components awarded on pickup (GDD: +1 per component).

    /// <summary>Fires the component collected event and destroys the pickup, called by PlayerCollisionController on overlap.</summary>
    public void Collect()
    {
        EventSystem.OnComponentCollected?.Invoke(_componentAmount);
        Destroy(gameObject);
    }
}