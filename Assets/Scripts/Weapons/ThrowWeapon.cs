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
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float projectileSize = 1f;
    [SerializeField] private int poolSize = 8;
    [SerializeField] private Transform muzzle;

    private ProjectileEmitter _emitter;

    public string DisplayName => displayName;

    private void Awake()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"ThrowWeapon '{name}': no projectile prefab assigned.");
            return;
        }

        _emitter = new ProjectileEmitter(projectilePrefab, projectileSpeed, projectileLifetime, projectileSize, poolSize, $"{name}_Pool");
    }

    public void Attack(Vector2 direction)
    {
        Vector2 origin = muzzle != null ? muzzle.position : transform.position;
        _emitter?.Fire(origin, direction);
    }
}
