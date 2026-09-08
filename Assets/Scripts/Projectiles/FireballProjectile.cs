using UnityEngine;

/// <summary>
/// An enemy fireball: flies straight in the direction it is fired (the base
/// <see cref="BaseProjectile"/> launch) and kills the player on contact via <see cref="IKillable"/>,
/// then despawns. Built and recycled through the same Builder/Factory/Pool as the player's
/// weapons — only the behaviour on hit differs (it harms the player, not enemies).
/// </summary>
public class FireballProjectile : BaseProjectile
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var killable = other.GetComponent<IKillable>();
        if (killable != null)
        {
            killable.Kill();
            Despawn();
        }
    }
}
