using UnityEngine;
using Zenject;

/// <summary>
/// A bonfire: touching it is <b>instant death</b> — it routes to the single
/// <see cref="IDeathService"/> so it stays ignorant of lives, respawn and level flow. Per the
/// brief a bonfire is destroyable only by the fairy, and hitting one while mounted sacrifices
/// the mount instead of killing the player; those paths arrive with the fairy (ME-64) and
/// mounts (ME-45).
/// </summary>
public class Bonfire : Hazard
{
    private IDeathService _death;

    [Inject]
    public void Construct(IDeathService death)
    {
        _death = death;
    }

    protected override void OnPlayerHit(GameObject player)
    {
        _death?.Die();
    }
}
