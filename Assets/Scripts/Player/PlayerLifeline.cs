using UnityEngine;
using Zenject;

/// <summary>
/// The player's <see cref="IKillable"/> endpoint: everything that kills the player on contact —
/// an enemy touch, a bonfire, an enemy fireball — calls <see cref="Kill"/>. What that does depends
/// on the player's state, checked in order:
/// <list type="number">
/// <item>fairy invincibility active → ignored (the player is unharmed);</item>
/// <item>riding a mount → the <b>mount</b> takes the hit and is lost (dismount), the player survives
/// with a brief grace window so the same enemy can't immediately finish them;</item>
/// <item>otherwise → a real death through the single <see cref="IDeathService"/>.</item>
/// </list>
/// Non-contact deaths (power running out, a pit) call <see cref="IDeathService"/> directly, so they
/// are unaffected by the mount — running out of power still kills you even while riding.
/// </summary>
public class PlayerLifeline : MonoBehaviour, IKillable
{
    [Tooltip("Seconds of invulnerability after a mount absorbs a hit, so you aren't instantly re-hit.")]
    [SerializeField] private float mountHitGraceSeconds = 1f;

    private IDeathService _death;
    private IInvincibility _invincibility;
    private PlayerMount _mount;
    private float _graceUntil;

    [Inject]
    public void Construct(IDeathService death, IInvincibility invincibility)
    {
        _death = death;
        _invincibility = invincibility;
    }

    private void Awake()
    {
        _mount = GetComponent<PlayerMount>();
    }

    public void Kill()
    {
        if (_invincibility != null && _invincibility.IsActive)
            return; // fairy: the player takes no contact damage

        if (Time.time < _graceUntil)
            return; // just lost a mount; brief window of safety

        if (_mount != null && _mount.IsMounted)
        {
            _mount.Dismount();               // the hit costs the mount, not a life
            _graceUntil = Time.time + mountHitGraceSeconds;
            return;
        }

        _death?.Die();
    }
}
