using UnityEngine;

/// <summary>
/// Ghost enemy: nothing destroys it except the fairy. Unlike the other enemies it is <b>not</b>
/// <see cref="IDamageable"/> at all — it extends <see cref="Enemy"/> directly rather than
/// <see cref="DamageableEnemy"/> — so weapons and mount attacks (which look for an
/// <see cref="IDamageable"/>) simply find nothing to hit and leave it alone. No empty "ignore damage"
/// override is needed; immunity falls out of the type instead of being faked (Interface Segregation
/// / Liskov). The fairy still removes it through the shared <see cref="Enemy.Kill"/> path (ME-64).
/// It drifts and bobs like a hovering ghost.
/// </summary>
public class Ghost : Enemy
{
    [SerializeField] private float driftSpeed = 1f;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float direction = -1f;

    private Vector3 _origin;
    private float _time;

    private void Awake()
    {
        _origin = transform.position;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float x = _origin.x + direction * driftSpeed * _time;
        float y = _origin.y + Mathf.Sin(_time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(x, y, _origin.z);
    }
}
