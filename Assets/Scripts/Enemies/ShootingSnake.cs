using UnityEngine;

/// <summary>
/// A snake that shoots a fireball at intervals. Extends <see cref="Enemy"/> for health, the
/// shared death path and player-contact damage, and reuses the projectile Builder/Factory/Pool
/// (via <see cref="ProjectileFactory"/> + <see cref="ProjectilePool"/>) to spawn fireballs — the
/// same pooling the player's weapons use. Pooled fireballs live under a runtime container so they
/// are never serialized into the scene.
/// </summary>
public class ShootingSnake : Enemy
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int poolSize = 4;
    [SerializeField] private float facing = -1f;

    private ProjectilePool _pool;
    private float _timer;

    protected override void Awake()
    {
        base.Awake();
        if (fireballPrefab == null)
        {
            Debug.LogError($"ShootingSnake '{name}': no fireball prefab assigned.");
            return;
        }

        var container = new GameObject($"{name}_FireballPool");
        var factory = new ProjectileFactory(fireballPrefab, projectileSpeed, projectileLifetime, projectileSize);
        _pool = new ProjectilePool(factory, poolSize, container.transform);
    }

    private void Update()
    {
        if (_pool == null)
            return;

        _timer += Time.deltaTime;
        if (_timer < fireInterval)
            return;

        _timer = 0f;
        BaseProjectile fireball = _pool.Get();
        if (fireball == null)
            return; // pool exhausted

        fireball.transform.position = transform.position;
        fireball.Fire(new Vector2(facing, 0f));
    }
}
