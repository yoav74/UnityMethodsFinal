using System;
using UnityEngine;

/// <summary>
/// <b>Model</b> of the player's power/energy meter. Owns the value and its rules (drain, add,
/// reset) and announces changes — no timing, UI or scene knowledge lives here. Starts full at
/// <see cref="GameConfig.StartingPower"/> and is capped there (fruits top it up, never over).
/// </summary>
public class PowerModel
{
    private readonly int _max;

    public int Power { get; private set; }
    public int Max => _max;

    /// <summary>Raised whenever the value changes, with the new amount.</summary>
    public event Action<int> PowerChanged;

    /// <summary>Raised the moment power reaches zero.</summary>
    public event Action PowerDepleted;

    public PowerModel(GameConfig config)
    {
        _max = config.StartingPower;
        Power = _max;
    }

    /// <summary>Remove power (time drain, or a rock hit). Clamped at zero.</summary>
    public void Drain(int amount)
    {
        if (Power <= 0)
            return;

        Power = Mathf.Max(0, Power - amount);
        PowerChanged?.Invoke(Power);

        if (Power <= 0)
            PowerDepleted?.Invoke();
    }

    /// <summary>Restore power (fruit). Clamped at <see cref="Max"/>.</summary>
    public void Add(int amount)
    {
        Power = Mathf.Min(_max, Power + amount);
        PowerChanged?.Invoke(Power);
    }

    /// <summary>Refill to full (respawn / new life).</summary>
    public void Reset()
    {
        Power = _max;
        PowerChanged?.Invoke(Power);
    }
}
