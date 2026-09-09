using UnityEngine;

/// <summary>
/// Frog enemy: sits still until the player comes within range, then leaps — high and far — in a
/// scripted parabola toward the player, with a cooldown between leaps. Extends
/// <see cref="DamageableEnemy"/> for health, the shared death path and player-contact damage, adding
/// only its movement (OCP).
/// </summary>
public class Frog : DamageableEnemy
{
    [SerializeField] private float detectRange = 4f;
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpDuration = 0.7f;
    [SerializeField] private float cooldown = 1f;
    // playerTag is inherited from Enemy (declaring it again would serialize the name twice).

    private Transform _player;
    private float _groundY;
    private float _z;
    private float _startX;
    private float _dir;
    private float _jumpTime = -1f; // < 0 when not jumping
    private float _cooldownLeft;

    protected override void Awake()
    {
        base.Awake();
        _groundY = transform.position.y;
        _z = transform.position.z;
        var playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        if (_jumpTime >= 0f)
        {
            Leap();
            return;
        }

        if (_cooldownLeft > 0f)
            _cooldownLeft -= Time.deltaTime;

        if (_player != null && _cooldownLeft <= 0f &&
            Mathf.Abs(_player.position.x - transform.position.x) <= detectRange)
        {
            _jumpTime = 0f;
            _startX = transform.position.x;
            _dir = Mathf.Sign(_player.position.x - transform.position.x);
            if (_dir == 0f) _dir = 1f;
        }
    }

    private void Leap()
    {
        _jumpTime += Time.deltaTime;
        float k = Mathf.Clamp01(_jumpTime / jumpDuration);
        float x = _startX + _dir * jumpDistance * k;
        float y = _groundY + jumpHeight * Mathf.Sin(Mathf.PI * k);
        transform.position = new Vector3(x, y, _z);

        if (k >= 1f)
        {
            transform.position = new Vector3(_startX + _dir * jumpDistance, _groundY, _z);
            _jumpTime = -1f;
            _cooldownLeft = cooldown;
        }
    }
}
