using UnityEngine;

/// <summary>
/// Snake that <b>really hops</b> along a platform: each hop is a velocity + gravity arc (not a
/// scripted sine that snaps back to a fixed height), so it rises, falls, and lands on whatever ground
/// is actually beneath it. Before launching it probes the landing spot — if there's a gap there it
/// turns around instead of hopping into empty air, so it patrols its ledge rather than hovering
/// across a gap. Its collider stays a trigger (that's how touching the player hurts them, see
/// <see cref="Enemy"/>), so ground is found with a downward raycast rather than physics collision.
/// Extends <see cref="DamageableEnemy"/> for health and the shared death/respawn path.
/// </summary>
public class JumpingSnake : DamageableEnemy
{
    [SerializeField] private float forwardSpeed = 3f;      // horizontal speed while hopping
    [SerializeField] private float jumpSpeed = 6f;         // upward launch speed
    [SerializeField] private float gravity = 16f;          // fall acceleration
    [SerializeField] private float pauseDuration = 0.5f;   // rest between hops
    [SerializeField] private float direction = -1f;        // -1 = left, +1 = right
    [SerializeField] private LayerMask groundMask = ~0;    // what counts as ground
    [SerializeField] private float edgeProbeDrop = 1.6f;   // how far below to still count as "ground ahead"

    private Collider2D _col;
    private float _vy;
    private bool _grounded;
    private float _pauseTimer;

    protected override void Awake()
    {
        base.Awake();
        _col = GetComponent<Collider2D>();
        _grounded = true;
        _pauseTimer = pauseDuration;
    }

    // A respawned snake starts fresh at its death spot: standing, ready to hop.
    protected override void OnEnable()
    {
        base.OnEnable();
        _vy = 0f;
        _grounded = true;
        _pauseTimer = pauseDuration;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (_grounded)
        {
            _pauseTimer -= dt;
            if (_pauseTimer > 0f)
                return;                          // sit still between hops

            // Turn around at a ledge edge instead of hopping into a gap.
            float airtime = 2f * jumpSpeed / gravity;
            float landX = transform.position.x + direction * forwardSpeed * airtime;
            if (!HasGroundAt(landX))
                direction = -direction;

            _vy = jumpSpeed;                     // launch the arc
            _grounded = false;
        }

        // Airborne: integrate the arc.
        _vy -= gravity * dt;
        Vector3 p = transform.position;
        p.x += direction * forwardSpeed * dt;
        p.y += _vy * dt;
        transform.position = p;

        // Land when descending onto ground beneath the feet.
        if (_vy <= 0f)
        {
            RaycastHit2D hit = Cast(_col.bounds.center.x, _col.bounds.min.y + 0.05f, 0.15f);
            if (hit.collider != null)
            {
                float bottomToPivot = transform.position.y - _col.bounds.min.y;
                p.y = hit.point.y + bottomToPivot;   // sit on the surface
                transform.position = p;
                _grounded = true;
                _vy = 0f;
                _pauseTimer = pauseDuration;
            }
        }
    }

    /// <summary>True if solid ground sits at world-x <paramref name="x"/>, within a hop's drop.</summary>
    private bool HasGroundAt(float x)
    {
        return Cast(x, _col.bounds.min.y + 0.05f, edgeProbeDrop).collider != null;
    }

    private RaycastHit2D Cast(float x, float y, float distance)
    {
        bool prev = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;    // ignore our own trigger and other triggers
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), Vector2.down, distance, groundMask);
        Physics2D.queriesHitTriggers = prev;
        return hit;
    }
}
