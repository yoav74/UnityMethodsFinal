using UnityEngine;
using Zenject;

/// <summary>
/// Makes the player jump on the jump button, but only when grounded — no mid-air or
/// infinite jumps (per the brief's "one button jump"). Grounded is a short downward
/// raycast from just below the collider's feet. Single responsibility: jumping; horizontal
/// movement and attack are separate components.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundCheckDistance = 0.12f;
    [Tooltip("Which layers count as ground. Default: everything.")]
    [SerializeField] private LayerMask groundMask = ~0;

    private IInputService _input;
    private Rigidbody2D _body;
    private Collider2D _collider;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_input.JumpPressed && IsGrounded())
            Jump();
    }

    /// <summary>True when a solid collider sits just beneath the player's feet.</summary>
    public bool IsGrounded()
    {
        Bounds bounds = _collider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.02f);
        return Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask).collider != null;
    }

    private void Jump()
    {
        _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f); // reset fall for a consistent height
        _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
