using UnityEngine;

/// <summary>
/// A snake that shoots a fireball at intervals. Extends <see cref="DamageableEnemy"/> for health,
/// the shared death path and player-contact damage, and reuses the projectile Builder/Factory/Pool
/// (via <see cref="ProjectileFactory"/> + <see cref="ProjectilePool"/>) to spawn fireballs — the
/// same pooling the player's weapons use. Pooled fireballs live under a runtime container so they
/// are never serialized into the scene.
/// </summary>
public class ShootingSnake : DamageableEnemy
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int poolSize = 4;
    [SerializeField] private float facing = -1f;

    private ProjectileEmitter _emitter;
    private float _timer;

    protected override void Awake()
    {
        base.Awake();
        if (fireballPrefab == null)
        {
            Debug.LogError($"ShootingSnake '{name}': no fireball prefab assigned.");
            return;
        }

        _emitter = new ProjectileEmitter(fireballPrefab, projectileSpeed, projectileLifetime, projectileSize, poolSize, $"{name}_FireballPool");
    }

    private void Update()
    {
        if (_emitter == null)
            return;

        _timer += Time.deltaTime;
        if (_timer < fireInterval)
            return;

        _timer = 0f;
        _emitter.Fire(transform.position, new Vector2(facing, 0f));
    }
}
