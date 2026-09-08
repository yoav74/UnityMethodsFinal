using UnityEngine;

/// <summary>
/// The red animal's fire spit: a straight-flying <see cref="BaseProjectile"/> that follows the
/// shared animal-attack rules on contact — it damages enemies (<see cref="IDamageable"/>) and
/// shatters rocks (<see cref="IDestructible"/>), but leaves the player and bonfires untouched —
/// then despawns. Built and recycled through the same Builder/Factory/Pool as every other
/// projectile.
/// </summary>
public class AnimalFireProjectile : BaseProjectile
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var damageable = other.GetComponent<IDamageable>();
        var destructible = other.GetComponent<IDestructible>();
        if (damageable == null && destructible == null)
            return; // passes over the player, bonfires, scenery

        damageable?.TakeDamage(1);
        destructible?.Hit();
        Despawn();
    }
}
