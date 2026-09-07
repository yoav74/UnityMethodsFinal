using UnityEngine;

/// <summary>
/// A weapon that throws pooled projectiles (hammer, boomerang, …). It owns one
/// <see cref="ProjectilePool"/> for its projectile type (built via the factory) and, on
/// <see cref="Attack"/>, pulls a projectile from the pool, places it at the muzzle and fires
/// it. Different weapons are just this component configured with a different projectile
/// prefab + tuning — no subclass needed (OCP).
/// </summary>
public class ThrowWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private string displayName = "Weapon";
    [SerializeField] private Sprite icon;
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int poolSize = 8;
    [SerializeField] private Transform muzzle;

    private ProjectilePool _pool;

    public string DisplayName => displayName;
    public Sprite Icon => icon;

    private void Awake()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"ThrowWeapon '{name}': no projectile prefab assigned.");
            return;
        }

        var factory = new ProjectileFactory(projectilePrefab, projectileSpeed, projectileLifetime, projectileSize);
        _pool = new ProjectilePool(factory, poolSize, transform);
    }

    public void Attack(Vector2 direction)
    {
        if (_pool == null)
            return;

        BaseProjectile projectile = _pool.Get();
        if (projectile == null)
            return; // pool exhausted

        projectile.transform.position = muzzle != null ? muzzle.position : transform.position;
        projectile.Fire(direction);
    }
}
