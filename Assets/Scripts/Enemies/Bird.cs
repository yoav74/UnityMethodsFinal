using UnityEngine;

/// <summary>
/// Bird enemy: flies steadily to the left while bobbing down and up along a sine path. Extends
/// <see cref="Enemy"/> for health, the shared death path and player-contact damage, adding only
/// its movement (Open/Closed).
/// </summary>
public class Bird : Enemy
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float bobAmplitude = 1f;
    [SerializeField] private float bobFrequency = 3f;

    private Vector3 _origin;
    private float _time;

    protected override void Awake()
    {
        base.Awake();
        _origin = transform.position;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float x = _origin.x - speed * _time;
        float y = _origin.y + Mathf.Sin(_time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(x, y, _origin.z);
    }
}
