using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <b>Object Pool</b> for projectiles: a fixed reserve of <see cref="BaseProjectile"/>
/// instances, created through the <see cref="ProjectileFactory"/> and reused instead of
/// Instantiate/Destroy per shot. <see cref="Get"/> hands one out (or null when exhausted —
/// it never grows); <see cref="Return"/> takes a spent one back. A weapon owns one pool per
/// projectile type; the projectile returns itself via <see cref="BaseProjectile.Despawn"/>.
/// </summary>
public class ProjectilePool : IProjectilePool
{
    private readonly ProjectileFactory _factory;
    private readonly Transform _parent;
    private readonly Queue<BaseProjectile> _available = new Queue<BaseProjectile>();

    public ProjectilePool(ProjectileFactory factory, int initialSize, Transform parent = null)
    {
        _factory = factory;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            BaseProjectile projectile = CreatePooled();
            if (projectile != null)
                _available.Enqueue(projectile);
        }
    }

    /// <summary>Takes a projectile from the pool, or null when the fixed reserve is exhausted.</summary>
    public BaseProjectile Get()
    {
        if (_available.Count == 0)
        {
            Debug.Log("ProjectilePool: empty - shot skipped");
            return null;
        }

        BaseProjectile projectile = _available.Dequeue();
        projectile.transform.SetParent(null);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    /// <summary>Returns a spent projectile to the pool for reuse.</summary>
    public void Return(BaseProjectile projectile)
    {
        if (projectile == null)
            return;

        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(_parent);
        _available.Enqueue(projectile);
    }

    private BaseProjectile CreatePooled()
    {
        BaseProjectile projectile = _factory.Create();
        if (projectile == null)
            return null;

        projectile.SetPool(this);
        projectile.gameObject.SetActive(false);
        projectile.transform.SetParent(_parent);
        return projectile;
    }
}
