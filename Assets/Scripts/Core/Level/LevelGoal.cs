using UnityEngine;
using Zenject;

/// <summary>
/// The end-of-level trigger: when the player reaches it, it tells the <see cref="ILevelLoader"/>
/// to advance to the next level. It knows nothing about scenes or which level is next — it just
/// signals "level complete" (Dependency Inversion). Place one at the end of each level.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private ILevelLoader _levels;
    private bool _reached;

    [Inject]
    public void Construct(ILevelLoader levels)
    {
        _levels = levels;
    }

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_reached || !other.CompareTag(playerTag))
            return;

        _reached = true; // only complete once
        _levels.LoadNext();
    }
}
