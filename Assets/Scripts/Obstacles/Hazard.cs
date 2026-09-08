using UnityEngine;

/// <summary>
/// Base for obstacles that affect the player on contact (bonfire, rock). It owns the shared
/// mechanics — a trigger collider and detecting the player by tag — and leaves the effect to
/// subclasses via <see cref="OnPlayerHit"/> (Open/Closed). Unlike a <see cref="Pickup"/> a
/// hazard is not consumed on contact; it persists until something is allowed to destroy it
/// (fairy, boomerang, a mount's attack — added with those features).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Hazard : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            OnPlayerHit(other.gameObject);
    }

    /// <summary>Apply this hazard's effect to the player that touched it.</summary>
    protected abstract void OnPlayerHit(GameObject player);
}
