using UnityEngine;

/// <summary>
/// Red animal mount: its attack <b>spits fire</b> — a pooled projectile fired in the facing
/// direction, reusing the projectile Builder/Factory/Pool (like the player's weapons and the
/// shooting snake). The fireball follows the shared animal rules (hurts enemies + rocks, not the
/// player or bonfires). Pooled fireballs live under a runtime container so they never serialize
/// into the scene. Collected via the leaf pickup (ME-49).
/// </summary>
public class RedAnimal : Animal
{
    [SerializeField] private GameObject fireProjectilePrefab;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int poolSize = 4;

    private ProjectilePool _pool;

    private void Awake()
    {
        if (fireProjectilePrefab == null)
        {
            Debug.LogError($"RedAnimal '{name}': no fire projectile prefab assigned.");
            return;
        }

        var container = new GameObject($"{name}_FirePool");
        var factory = new ProjectileFactory(fireProjectilePrefab, projectileSpeed, projectileLifetime, projectileSize);
        _pool = new ProjectilePool(factory, poolSize, container.transform);
    }

    public override void Attack(Vector2 direction)
    {
        if (_pool == null)
            return;

        BaseProjectile fire = _pool.Get();
        if (fire == null)
            return; // pool exhausted

        fire.transform.position = transform.position;
        fire.Fire(direction);
    }
}
