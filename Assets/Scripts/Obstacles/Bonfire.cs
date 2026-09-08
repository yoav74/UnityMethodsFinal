using UnityEngine;

/// <summary>
/// A bonfire: touching it kills the player — it routes through the player's <see cref="IKillable"/>
/// so the fairy's invincibility can spare them. Per the brief it is destroyable <b>only by the
/// fairy</b>, so it implements <see cref="IFairyDestructible"/> (not <see cref="IDestructible"/>) —
/// weapons and animal attacks leave it standing. The mounted "sacrifice the mount" path arrives
/// with mounts (ME-45/ME-50).
/// </summary>
public class Bonfire : Hazard, IFairyDestructible
{
    protected override void OnPlayerHit(GameObject player)
    {
        player.GetComponent<IKillable>()?.Kill();
    }

    /// <summary>Destroyed by the fairy only.</summary>
    public void DestroyByFairy()
    {
        Destroy(gameObject);
    }
}
