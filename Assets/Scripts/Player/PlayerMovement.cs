using UnityEngine;
using Zenject;

/// <summary>
/// Drives the player's horizontal locomotion from the injected <see cref="IInputService"/>
/// and flips the sprite to face the way it travels. Physics-based (Rigidbody2D) so it
/// collides with the ground and level obstacles. Single responsibility: horizontal movement
/// and facing — jump and attack are separate components (ME-32 / ME-33), which can read
/// <see cref="FacingRight"/> to fire the right way.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;

    private IInputService _input;
    private Rigidbody2D _body;
    private SpriteRenderer _sprite;

    /// <summary>Which way the player currently faces. Weapons/mounts read this to aim.</summary>
    public bool FacingRight { get; private set; } = true;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float axis = _input.MoveAxis;
        _body.linearVelocity = new Vector2(axis * moveSpeed, _body.linearVelocity.y);

        if (axis > 0.01f)
            SetFacing(true);
        else if (axis < -0.01f)
            SetFacing(false);
    }

    private void SetFacing(bool right)
    {
        FacingRight = right;
        if (_sprite != null)
            _sprite.flipX = !right; // art faces right by default; flip when moving left
    }
}
