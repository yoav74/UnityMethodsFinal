using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The weapons the player has picked up and which one is active. Single responsibility: own
/// that state. It does not read input or fire — the player's attack (ME-33) asks
/// <see cref="Current"/> to attack, and pickups call <see cref="Add"/>.
/// </summary>
public class WeaponInventory : MonoBehaviour
{
    private readonly List<IWeapon> _weapons = new List<IWeapon>();

    /// <summary>The weapon an attack currently uses, or null if none collected.</summary>
    public IWeapon Current { get; private set; }

    public bool HasWeapon => Current != null;

    public void Add(IWeapon weapon)
    {
        if (weapon == null || _weapons.Contains(weapon))
            return;

        _weapons.Add(weapon);
        Current ??= weapon; // auto-equip the first weapon collected
    }

    /// <summary>Cycle to the next collected weapon (if the player carries more than one).</summary>
    public void SelectNext()
    {
        if (_weapons.Count == 0)
            return;

        int index = _weapons.IndexOf(Current);
        Current = _weapons[(index + 1) % _weapons.Count];
    }
}
