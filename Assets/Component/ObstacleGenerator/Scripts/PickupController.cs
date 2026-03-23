using UnityEngine;

/// <summary>Heals the player on collection and destroys itself when picked up.</summary>
public class PickupController : MonoBehaviour
{
    [SerializeField] private float _healAmount = 8f; // HP restored on pickup (GDD: +8 HP per common waste).

    /// <summary>Fires the heal event and destroys the pickup, called by PlayerCollisionController on overlap.</summary>
    public void Collect()
    {
        EventSystem.OnPlayerHealed?.Invoke(_healAmount);
        Destroy(gameObject);
    }
}