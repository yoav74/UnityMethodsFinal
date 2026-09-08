using UnityEngine;
using Zenject;

/// <summary>
/// The player's <see cref="IKillable"/> endpoint: everything that kills the player on contact —
/// an enemy touch, a bonfire, an enemy fireball — calls <see cref="Kill"/>, and this forwards to
/// the single <see cref="IDeathService"/>. While the fairy's invincibility is active it ignores
/// those contact deaths (the player is unharmed). Non-contact deaths (power running out, a pit)
/// go straight through <see cref="IDeathService"/> and are unaffected.
/// </summary>
public class PlayerLifeline : MonoBehaviour, IKillable
{
    private IDeathService _death;
    private IInvincibility _invincibility;

    [Inject]
    public void Construct(IDeathService death, IInvincibility invincibility)
    {
        _death = death;
        _invincibility = invincibility;
    }

    public void Kill()
    {
        if (_invincibility != null && _invincibility.IsActive)
            return; // fairy: the player takes no contact damage

        _death?.Die();
    }
}
