using UnityEngine;

/// <summary>
/// <b>Template Method</b> base for every projectile (hammer, boomerang, animal fire, snake
/// fireball). <see cref="Fire"/> defines the fixed firing sequence — resolve direction →
/// prepare → launch → schedule despawn → post-fire — and subclasses fill in the individual
/// steps through the protected hooks. Subclasses never override <see cref="Fire"/>, so a new
/// projectile is added by extension, not by changing this class (Open/Closed).
///
/// A projectile can belong to an <see cref="IProjectilePool"/>: when spent it returns itself
/// via <see cref="Despawn"/>, or destroys itself if it has no pool.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float lifetime = 3f;

    protected Rigidbody2D body;
    private IProjectilePool _pool;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>Assigns the pool this projectile returns to when spent (set by the pool).</summary>
    public void SetPool(IProjectilePool pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// Template method — the invariant firing sequence. <paramref name="aim"/> is the
    /// requested direction (a fixed-direction projectile ignores it via ResolveDirection).
    /// </summary>
    public void Fire(Vector2 aim)
    {
        Vector2 direction = ResolveDirection(aim);
        Prepare(direction);
        Launch(direction);
        ScheduleDespawn();
        OnFired(direction);
    }

    public void Fire() => Fire(Vector2.right);

    /// <summary>Set motion params in code (used by the builder).</summary>
    public void Configure(float projectileSpeed, float projectileLifetime)
    {
        speed = projectileSpeed;
        lifetime = projectileLifetime;
    }

    /// <summary>Remove from play: return to the pool if pooled, otherwise destroy.</summary>
    public void Despawn()
    {
        CancelInvoke(nameof(Despawn));
        if (_pool != null)
            _pool.Return(this);
        else
            Destroy(gameObject);
    }

    // ---- hooks the subclasses fill in ----

    protected virtual Vector2 ResolveDirection(Vector2 aim) => aim;

    protected virtual void Prepare(Vector2 direction)
    {
        CancelInvoke(nameof(Despawn));
        if (body != null)
            body.linearVelocity = Vector2.zero;
    }

    protected virtual void Launch(Vector2 direction)
    {
        if (body != null)
            body.linearVelocity = direction.normalized * speed;
    }

    protected virtual void ScheduleDespawn()
    {
        if (lifetime > 0f)
            Invoke(nameof(Despawn), lifetime);
    }

    protected virtual void OnFired(Vector2 direction) { }
}
