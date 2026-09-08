using UnityEngine;

/// <summary>
/// <b>Builder</b> for projectiles: assembles a <see cref="BaseProjectile"/> one step at a
/// time (speed, lifetime, size, sprite) and produces the finished instance from a supplied
/// prefab. Separates *how* a projectile is put together from the code that just wants one —
/// callers use the <see cref="ProjectileDirector"/> or the factory instead of these steps.
/// </summary>
public class ProjectileBuilder
{
    private readonly GameObject _prefab;

    private float _speed = 10f;
    private float _lifetime = 3f;
    private float _size = 1f;
    private Sprite _sprite;

    /// <param name="prefab">A prefab carrying a <see cref="BaseProjectile"/> (plus its
    /// Rigidbody2D / Collider2D / SpriteRenderer). The built projectile keeps the prefab's
    /// physics/collider setup; the steps below override its configurable values.</param>
    public ProjectileBuilder(GameObject prefab)
    {
        _prefab = prefab;
    }

    public ProjectileBuilder SetSpeed(float speed) { _speed = speed; return this; }
    public ProjectileBuilder SetLifetime(float lifetime) { _lifetime = lifetime; return this; }
    public ProjectileBuilder SetSize(float size) { _size = size; return this; }
    public ProjectileBuilder SetSprite(Sprite sprite) { _sprite = sprite; return this; }

    /// <summary>Instantiates the prefab and applies the configured steps to it.</summary>
    public BaseProjectile Build()
    {
        if (_prefab == null)
        {
            Debug.LogError("ProjectileBuilder: no prefab provided.");
            return null;
        }

        GameObject instance = Object.Instantiate(_prefab);
        BaseProjectile projectile = instance.GetComponent<BaseProjectile>();
        if (projectile == null)
        {
            Debug.LogError("ProjectileBuilder: prefab has no BaseProjectile component.");
            Object.Destroy(instance);
            return null;
        }

        projectile.Configure(_speed, _lifetime);
        instance.transform.localScale = Vector3.one * _size;

        if (_sprite != null)
        {
            SpriteRenderer renderer = instance.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
                renderer.sprite = _sprite;
        }

        return projectile;
    }
}
