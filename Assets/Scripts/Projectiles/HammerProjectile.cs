using UnityEngine;

/// <summary>
/// The hammer projectile: thrown in an arc in the facing direction and tumbling in flight,
/// like the reference game. It fills in the <see cref="BaseProjectile"/> hooks — an arced
/// launch impulse and a spin — without touching the shared firing sequence.
/// </summary>
public class HammerProjectile : BaseProjectile
{
    [SerializeField] private float horizontalSpeed = 8f;
    [SerializeField] private float verticalSpeed = 7f;
    [SerializeField] private float spinSpeed = 720f; // degrees/second

    private float spinDirection = -1f;
    private bool thrown;

    protected override void Prepare(Vector2 direction)
    {
        base.Prepare(direction);
        thrown = false;
        transform.rotation = Quaternion.identity;
        float facing = direction.x < 0f ? -1f : 1f;
        spinDirection = -facing; // tumble in the direction of travel
    }

    protected override void Launch(Vector2 direction)
    {
        float facing = direction.x < 0f ? -1f : 1f;
        if (body != null)
            body.AddForce(new Vector2(facing * horizontalSpeed, verticalSpeed), ForceMode2D.Impulse);
    }

    protected override void OnFired(Vector2 direction)
    {
        thrown = true;
    }

    private void Update()
    {
        if (thrown)
            transform.Rotate(0f, 0f, spinDirection * spinSpeed * Time.deltaTime);
    }
}
