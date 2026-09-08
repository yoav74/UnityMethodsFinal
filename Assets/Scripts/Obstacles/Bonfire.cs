using UnityEngine;

/// <summary>
/// A bonfire: touching it kills the player — it routes through the player's <see cref="IKillable"/>
/// so the fairy's invincibility can spare them (and, while invincible, the fairy aura destroys the
/// bonfire instead). It is <see cref="IDestructible"/> so the fairy can remove it; the mounted
/// "sacrifice the mount" path arrives with mounts (ME-45).
/// </summary>
public class Bonfire : Hazard, IDestructible
{
    protected override void OnPlayerHit(GameObject player)
    {
        player.GetComponent<IKillable>()?.Kill();
    }

    /// <summary>Destroyed by the fairy.</summary>
    public void Hit()
    {
        Destroy(gameObject);
    }
}
