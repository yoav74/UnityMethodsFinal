using UnityEngine;

/// <summary>
/// Makes a spawned collectible <b>drop to the ground</b> instead of hanging in mid-air where the
/// egg opened or the enemy died. It casts straight down for the nearest ground, then eases the
/// object down under gravity until it rests on top of it. It uses a raycast rather than a
/// <see cref="Rigidbody2D"/> on purpose: pickups carry <i>trigger</i> colliders (so the player walks
/// into them), and triggers don't collide with the floor — a raycast settles them without turning
/// the pickup into a physics body or adding a second collider.
///
/// It can be authored on a prefab (it falls on <see cref="Start"/>) or bolted on at spawn time via
/// <see cref="Attach"/> — which is how <see cref="DropOnDeath"/> and the egg's
/// <see cref="CollectibleFactory"/> make every drop fall without each pickup prefab needing setup.
/// </summary>
public class FallToGround : MonoBehaviour
{
    [Tooltip("Which layers count as ground. Default (Everything) is fine for a simple level.")]
    [SerializeField] private LayerMask groundMask = ~0;

    [SerializeField] private float gravity = 25f;
    [SerializeField] private float maxFallSpeed = 20f;
    [SerializeField] private float castDistance = 50f;

    private float _velocity;
    private float _targetY;
    private bool _falling;

    /// <summary>Add (or reuse) the component on a freshly spawned object and start it falling.</summary>
    public static void Attach(GameObject target)
    {
        if (target == null)
            return;

        var fall = target.GetComponent<FallToGround>();
        if (fall == null)
            fall = target.AddComponent<FallToGround>();

        fall.Begin();
    }

    private void Start()
    {
        // Author-on-prefab path: fall once the object comes to life. (Attach() may have run first;
        // Begin() is safe to call again — it just re-measures from the current position.)
        Begin();
    }

    private void Begin()
    {
        // Start the ray just below our own collider so a trigger collider on this object can't be
        // what the cast hits first.
        var col = GetComponent<Collider2D>();
        float halfHeight = col != null ? col.bounds.extents.y : 0f;
        Vector2 origin = (Vector2)transform.position + Vector2.down * (halfHeight + 0.02f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, castDistance, groundMask);
        if (hit.collider == null)
        {
            _falling = false; // nothing below — leave it where it is rather than fall forever
            return;
        }

        _targetY = hit.point.y + halfHeight;
        _falling = transform.position.y > _targetY; // only fall if we're actually above the ground
        _velocity = 0f;
    }

    private void Update()
    {
        if (!_falling)
            return;

        _velocity = Mathf.Min(_velocity + gravity * Time.deltaTime, maxFallSpeed);

        Vector3 position = transform.position;
        position.y -= _velocity * Time.deltaTime;

        if (position.y <= _targetY)
        {
            position.y = _targetY;
            _falling = false; // landed
        }

        transform.position = position;
    }
}
