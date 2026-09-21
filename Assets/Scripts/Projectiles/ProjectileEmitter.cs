using UnityEngine;

/// <summary>
/// A reusable projectile emitter: owns one <see cref="ProjectilePool"/> of a projectile type (built
/// through the Builder/Factory) and fires an instance from a given point. The player's weapons, the
/// fire animal and the shooting snake all <b>compose</b> one of these instead of each repeating the
/// pool-setup and the get → place → fire steps, so that logic lives in a single place (DRY) and the
/// callers no longer reach through the factory/pool concretes themselves.
/// </summary>
public class ProjectileEmitter
{
    private readonly ProjectilePool _pool;

    /// <param name="containerName">Name for the runtime GameObject the reserve is parked under, so
    /// pooled instances are never serialized into the scene.</param>
    public ProjectileEmitter(GameObject prefab, float speed, float lifetime, float size, int poolSize, string containerName)
    {
        var container = new GameObject(containerName);
        var factory = new ProjectileFactory(prefab, speed, lifetime, size);
        _pool = new ProjectilePool(factory, poolSize, container.transform);
    }

    /// <summary>Pull a projectile from the pool, place it at <paramref name="origin"/> and fire it in
    /// <paramref name="direction"/>. A no-op when the pool is exhausted.</summary>
    public void Fire(Vector2 origin, Vector2 direction)
    {
        BaseProjectile projectile = _pool.Get();
        if (projectile == null)
            return; // pool exhausted

        projectile.transform.position = origin;
        projectile.Fire(direction);
    }
}
