using UnityEngine;

/// <summary>
/// Ghost enemy: nothing destroys it except the fairy. It overrides <see cref="Enemy.TakeDamage"/>
/// to ignore weapons and mount attacks entirely (Open/Closed — the base and every weapon stay
/// unchanged), while still dying through the shared path when the fairy calls <see cref="Enemy.Kill"/>
/// (ME-64). It drifts and bobs like a hovering ghost.
/// </summary>
public class Ghost : Enemy
{
    [SerializeField] private float driftSpeed = 1f;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float direction = -1f;

    private Vector3 _origin;
    private float _time;

    protected override void Awake()
    {
        base.Awake();
        _origin = transform.position;
    }

    /// <summary>A ghost is immune to ordinary damage; only the fairy (via <see cref="Enemy.Kill"/>) removes it.</summary>
    public override void TakeDamage(int amount)
    {
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float x = _origin.x + direction * driftSpeed * _time;
        float y = _origin.y + Mathf.Sin(_time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(x, y, _origin.z);
    }
}
