using UnityEngine;
using Zenject;

/// <summary>
/// A trigger strip placed below a level: falling into a pit and dropping in here is an instant
/// death, the way pits work in Adventure Island. It goes straight to <see cref="IDeathService"/>
/// (not the player's <see cref="IKillable"/>), so a pit kills regardless of a mount or fairy
/// invincibility — those save you from hits, not from falling off the level. Single responsibility:
/// turn "left the playfield" into a death.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private IDeathService _death;

    [Inject]
    public void Construct(IDeathService death)
    {
        _death = death;
    }

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            _death?.Die();
    }
}
