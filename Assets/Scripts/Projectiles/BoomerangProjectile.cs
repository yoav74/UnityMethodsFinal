using UnityEngine;

/// <summary>
/// The boomerang: flies <b>straight out</b> in the facing direction to a maximum distance, then
/// <b>arcs up and curves back toward the player</b>, homing onto them — so if the player has moved on
/// it chases them until it's caught. Spins the whole way and strikes on both legs. It fills in the
/// <see cref="BaseProjectile"/> hooks (a gravity-free launch, a distance-based turn, homing return)
/// without touching the shared firing sequence. On contact it destroys <see cref="IDestructible"/>
/// scenery and damages enemies; reaching the player catches it (despawn).
/// </summary>
public class BoomerangProjectile : BaseProjectile
{
    [SerializeField] private float spinSpeed = 720f;      // degrees/second
    [SerializeField] private float maxDistance = 6f;      // how far out before it turns back
    [SerializeField] private float returnUpBoost = 4f;    // upward kick as it turns ("goes up")
    [SerializeField] private float homingAccel = 40f;     // how fast the return velocity steers to the player
    [SerializeField] private float catchRadius = 0.6f;    // caught when this close to the player
    [SerializeField] private float maxChaseSeconds = 8f;  // safety despawn if the player is unreachable
    [SerializeField] private string playerTag = "Player";

    private Vector2 _outward;
    private Vector3 _start;
    private Transform _player;
    private bool _returning;
    private bool _flying;

    protected override void Prepare(Vector2 direction)
    {
        base.Prepare(direction);
        transform.rotation = Quaternion.identity;
        if (body != null)
            body.gravityScale = 0f;                       // the boomerang isn't affected by gravity
        _returning = false;
        _flying = false;
        _start = transform.position;

        var player = GameObject.FindWithTag(playerTag);
        _player = player != null ? player.transform : null;
    }

    protected override void Launch(Vector2 direction)
    {
        _outward = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        if (body != null)
            body.linearVelocity = _outward * speed;
    }

    // Use a generous safety timeout rather than the short weapon lifetime, since the return leg
    // chases a moving player; the catch is the normal way it ends.
    protected override void ScheduleDespawn()
    {
        CancelInvoke(nameof(Despawn));
        Invoke(nameof(Despawn), maxChaseSeconds);
    }

    protected override void OnFired(Vector2 direction)
    {
        _flying = true;
    }

    private void FixedUpdate()
    {
        if (!_flying || body == null)
            return;

        if (!_returning)
        {
            // Outbound: straight until it reaches its maximum reach.
            if (Vector2.Distance(transform.position, _start) >= maxDistance)
            {
                _returning = true;
                body.linearVelocity = (-_outward * speed) + (Vector2.up * returnUpBoost); // turn: back + up
            }
            return;
        }

        // Return: home onto the player's current position, so it follows if they moved on.
        if (_player != null)
        {
            Vector2 toPlayer = (Vector2)_player.position - (Vector2)transform.position;
            if (toPlayer.magnitude <= catchRadius)
            {
                Despawn();
                return;
            }

            Vector2 desired = toPlayer.normalized * speed;
            body.linearVelocity = Vector2.MoveTowards(body.linearVelocity, desired, homingAccel * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (_flying)
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_returning && other.CompareTag(playerTag))
        {
            Despawn();                                     // caught on the way back
            return;
        }

        other.GetComponent<IDestructible>()?.Hit();        // rocks
        other.GetComponent<IDamageable>()?.TakeDamage(1);  // enemies
    }
}
