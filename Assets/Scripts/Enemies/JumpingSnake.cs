using UnityEngine;

/// <summary>
/// Snake that hops forward in arcs along the ground. Extends <see cref="DamageableEnemy"/> for
/// health, the shared death path and player-contact damage, adding only its movement (Open/Closed). The hop
/// is a scripted parabola (kinematic), advancing one hop distance each cycle with a pause between
/// hops, so it never needs ground physics.
/// </summary>
public class JumpingSnake : DamageableEnemy
{
    [SerializeField] private float hopDistance = 2f;
    [SerializeField] private float hopHeight = 1.2f;
    [SerializeField] private float hopDuration = 0.6f;
    [SerializeField] private float pauseDuration = 0.6f;
    [SerializeField] private float direction = -1f; // -1 = forward (left)

    private float _baseX;
    private float _baseY;
    private float _z;
    private float _time;

    protected override void Awake()
    {
        base.Awake();
        _baseX = transform.position.x;
        _baseY = transform.position.y;
        _z = transform.position.z;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float cycle = hopDuration + pauseDuration;
        int completed = (int)(_time / cycle);
        float phase = _time - completed * cycle;
        float landedX = _baseX + direction * hopDistance * completed;

        if (phase <= hopDuration)
        {
            float k = phase / hopDuration; // 0..1 across the arc
            float x = landedX + direction * hopDistance * k;
            float y = _baseY + hopHeight * Mathf.Sin(Mathf.PI * k);
            transform.position = new Vector3(x, y, _z);
        }
        else
        {
            transform.position = new Vector3(landedX + direction * hopDistance, _baseY, _z);
        }
    }
}
