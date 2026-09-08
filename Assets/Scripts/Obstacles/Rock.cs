using UnityEngine;
using Zenject;

/// <summary>
/// A rock: touching it drains power by <see cref="GameConfig.RockPowerCost"/> (the brief: −3).
/// It reads the configured cost and the power meter through injection, so it stays ignorant of
/// how power works. It is <see cref="IDestructible"/> — the boomerang destroys it on contact
/// (a mount's attack will too, ME-45); the mounted "both disappear" path arrives with mounts.
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
        _power?.Drain(_powerCost);
    }

    /// <summary>Destroyed by a boomerang or a mount's attack.</summary>
    public void Hit()
    {
        Destroy(gameObject);
    }
}
