using UnityEngine;
using Zenject;

/// <summary>
/// The player's <see cref="IKillable"/> endpoint: anything that kills the player on contact
/// (an enemy fireball now; hazards could route here too) calls <see cref="Kill"/>, and this
/// forwards to the single <see cref="IDeathService"/>. Keeps killers ignorant of lives/respawn.
/// </summary>
public class PlayerLifeline : MonoBehaviour, IKillable
{
    private IDeathService _death;

    [Inject]
    public void Construct(IDeathService death)
    {
        _death = death;
    }

    public void Kill()
    {
        _death?.Die();
    }
}
