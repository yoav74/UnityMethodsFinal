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

    private ProjectileEmitter _emitter;

    protected override void Awake()
    {
        base.Awake();
        if (fireProjectilePrefab == null)
        {
            Debug.LogError($"RedAnimal '{name}': no fire projectile prefab assigned.");
            return;
        }

        _emitter = new ProjectileEmitter(fireProjectilePrefab, projectileSpeed, projectileLifetime, projectileSize, poolSize, $"{name}_FirePool");
    }

    protected override void PerformAttack(Vector2 direction)
    {
        // This animal's own strike shape: a pooled fireball in the facing direction.
        _emitter?.Fire(transform.position, direction);
    }
}
