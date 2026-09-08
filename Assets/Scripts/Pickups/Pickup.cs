using UnityEngine;

/// <summary>
/// Base for anything the player collects by walking into it (weapons, fruit, eggs, the fairy).
/// It owns the shared mechanics — a trigger collider, detecting the player by tag, firing once,
/// and removing itself — and leaves only the <i>effect</i> to subclasses via
/// <see cref="OnCollected"/>. New collectibles extend this without touching it (Open/Closed).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool _consumed;

    // When the component is first added in the editor, make the collider a trigger by default.
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed || !other.CompareTag(playerTag))
            return;

        _consumed = true; // guard against a second trigger before the object is destroyed
        OnCollected(other.gameObject);
        Destroy(gameObject);
    }

    /// <summary>Apply this pickup's effect to the player that collected it.</summary>
    protected abstract void OnCollected(GameObject player);
}
