using UnityEngine;
using Zenject;

/// <summary>
/// A rock: touching it drains power by <see cref="GameConfig.RockPowerCost"/> (the brief: −3).
/// It reads the configured cost and the power meter through injection, so it stays ignorant of
/// how power works. Per the brief a rock is destroyable by the boomerang or a mount's attack,
/// and hitting one while mounted removes both the mount and the rock; those paths arrive with
/// the boomerang (ME-44) and mounts (ME-45).
/// </summary>
public class Rock : Hazard
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
}
