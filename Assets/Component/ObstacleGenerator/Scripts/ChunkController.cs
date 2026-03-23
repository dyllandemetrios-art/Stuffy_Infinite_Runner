using UnityEngine;

/// <summary>Represents a single level chunk, exposing its end position and whether it has passed the player.</summary>
public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform _endAnchor; // Transform marking the end of this chunk, used to chain the next spawn.
    
    public Vector3 EndAnchor => _endAnchor.position; // World position of the chunk's end, readable by the spawn system.

    /// <summary>Returns true when the chunk's end has passed the player's position on the Z axis.</summary>
    public bool IsBehindPlayer()
    {
        return EndAnchor.z <= 0;
    }
}