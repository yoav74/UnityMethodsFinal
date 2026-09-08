using UnityEngine;

/// <summary>
/// A bonfire: on foot, touching it kills the player through <see cref="IKillable"/> (so the
/// fairy's invincibility can spare them). While <b>mounted</b>, riding into it instead sacrifices
/// the mount — the animal and the bonfire both disappear and the player survives, dismounted
/// (ME-50). It is destroyable <b>only by the fairy</b> (<see cref="IFairyDestructible"/>) —
/// weapons and animal attacks leave it standing.
/// </summary>
public class Bonfire : Hazard, IFairyDestructible
{
    protected override void OnPlayerHit(GameObject player)
    {
        var mount = player.GetComponent<PlayerMount>();
        if (mount != null && mount.IsMounted)
        {
            mount.Dismount(); // both the animal and the bonfire disappear; the player survives
            Destroy(gameObject);
            return;
        }

        player.GetComponent<IKillable>()?.Kill();
    }

    /// <summary>Destroyed by the fairy only.</summary>
    public void DestroyByFairy()
    {
        Destroy(gameObject);
    }
}
