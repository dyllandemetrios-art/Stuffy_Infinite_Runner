using UnityEngine;

/// <summary>Represents a single level chunk, exposing its end position, spawn weight, and whether it has passed the player.</summary>
public class ChunkController : MonoBehaviour
{
    [SerializeField] private Transform _endAnchor;  // Transform marking the end of this chunk, used to chain the next spawn.
    [SerializeField] private int _spawnWeight = 10; // Probability weight for this chunk (higher = more frequent).

    public Vector3 EndAnchor => _endAnchor.position; // World position of the chunk's end, readable by the spawn system.
    public int SpawnWeight => _spawnWeight;           // Weight used by ObstacleController for weighted random selection.

    /// <summary>Returns true when the chunk's end has passed the player's position on the Z axis.</summary>
    public bool IsBehindPlayer()
    {
        return EndAnchor.z <= 0;
    }
}