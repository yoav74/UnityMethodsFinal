using UnityEngine;
using Zenject;

/// <summary>
/// A rock: on foot, touching it drains power by <see cref="GameConfig.RockPowerCost"/> (the
/// brief: −3). While <b>mounted</b>, riding into it instead sacrifices the mount — the animal and
/// the rock both disappear and the player survives, dismounted (ME-50). It is
/// <see cref="IDestructible"/> so the boomerang and animal attacks shatter it; bonfires, by
/// contrast, are not.
/// </summary>
public class Rock : Hazard, IDestructible
{
    private PowerModel _power;
    private int _powerCost;

    [Inject]
    public void Construct(PowerModel power, GameConfig config)
    {
        _power = power;
        _powerCost = config.RockPowerCost;
    }

    protected override void OnPlayerHit(GameObject player)
    {
        var mount = player.GetComponent<PlayerMount>();
        if (mount != null && mount.IsMounted)
        {
            mount.Dismount(); // both the animal and the rock disappear; the player survives
            Destroy(gameObject);
            return;
        }

        _power?.Drain(_powerCost);
    }

    /// <summary>Destroyed by a boomerang or a mount's attack.</summary>
    public void Hit()
    {
        Destroy(gameObject);
    }
}
