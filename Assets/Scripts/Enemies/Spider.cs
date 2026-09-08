using UnityEngine;

/// <summary>
/// Spider enemy: patrols up and down around its start position, or hangs static in the air
/// (per-instance toggle). It extends <see cref="Enemy"/> for health, the shared death path and
/// player-contact damage, adding only its movement (Open/Closed). Movement is a sine sweep about
/// the spawn point, so it returns cleanly to origin.
/// </summary>
public class Spider : Enemy
{
    [SerializeField] private bool moveVertically = true;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float range = 2f;

    private Vector3 _origin;
    private float _time;

    protected override void Awake()
    {
        base.Awake();
        _origin = transform.position;
    }

    private void Update()
    {
        if (!moveVertically)
            return;

        _time += Time.deltaTime;
        float y = _origin.y + Mathf.Sin(_time * speed) * range;
        transform.position = new Vector3(_origin.x, y, _origin.z);
    }
}
