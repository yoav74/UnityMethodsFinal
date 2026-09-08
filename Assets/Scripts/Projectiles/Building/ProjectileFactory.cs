using UnityEngine;

/// <summary>
/// <b>Factory</b> for projectiles: hands out a ready-to-fire <see cref="BaseProjectile"/>
/// from a single <see cref="Create"/> call, hiding the builder + director assembly from
/// callers (weapons, pools). One factory is configured per projectile type — its prefab and
/// tuning — and <see cref="Create"/> produces a fresh, configured instance each time.
/// </summary>
public class ProjectileFactory
{
    private readonly ProjectileDirector _director;
    private readonly float _speed;
    private readonly float _lifetime;
    private readonly float _size;

    public ProjectileFactory(GameObject prefab, float speed, float lifetime, float size)
    {
        _director = new ProjectileDirector(new ProjectileBuilder(prefab));
        _speed = speed;
        _lifetime = lifetime;
        _size = size;
    }

    /// <summary>Creates a ready, fully configured projectile.</summary>
    public BaseProjectile Create()
    {
        return _director.Build(_speed, _lifetime, _size);
    }
}
