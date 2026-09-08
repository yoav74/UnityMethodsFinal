using UnityEngine;

/// <summary>
/// The boomerang projectile: flown straight out in the facing direction and back to the thrower,
/// spinning the whole way, and it can strike on both legs. It fills in the
/// <see cref="BaseProjectile"/> hooks — a gravity-free outward launch and a mid-flight reversal —
/// without touching the shared firing sequence. On contact it destroys anything
/// <see cref="IDestructible"/> (rocks now, enemies later); the hammer, by contrast, cannot.
/// </summary>
public class BoomerangProjectile : BaseProjectile
{
    [SerializeField] private float spinSpeed = 720f; // degrees/second

    private Vector2 _outward;
    private float _elapsed;
    private bool _returning;
    private bool _flying;

    protected override void Prepare(Vector2 direction)
    {
        base.Prepare(direction);
        transform.rotation = Quaternion.identity;
        _elapsed = 0f;
        _returning = false;
        _flying = false;
    }

    protected override void Launch(Vector2 direction)
    {
        _outward = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        if (body != null)
            body.linearVelocity = _outward * speed;
    }

    protected override void OnFired(Vector2 direction)
    {
        _flying = true;
    }

    private void FixedUpdate()
    {
        if (!_flying)
            return;

        _elapsed += Time.fixedDeltaTime;

        // Reverse at the midpoint so the outbound and return legs are symmetric and it comes
        // back to the thrower.
        if (!_returning && _elapsed >= lifetime * 0.5f)
        {
            _returning = true;
            if (body != null)
                body.linearVelocity = -_outward * speed;
        }
    }

    private void Update()
    {
        if (_flying)
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<IDestructible>()?.Hit();       // rocks
        other.GetComponent<IDamageable>()?.TakeDamage(1);  // enemies
    }
}
