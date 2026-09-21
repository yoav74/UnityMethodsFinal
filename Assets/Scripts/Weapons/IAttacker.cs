using UnityEngine;

/// <summary>
/// Something the player can attack with — a weapon or a mount. This is the whole of what the
/// player's attack depends on: it calls <see cref="Attack"/> and reads neither a name nor an icon,
/// so it depends only on this (Interface Segregation). <see cref="IWeapon"/> and <see cref="IMount"/>
/// extend it, letting the attack treat both alike (Open/Closed, DIP).
/// </summary>
public interface IAttacker
{
    /// <summary>Attack in the given aim direction (e.g. the way the player faces).</summary>
    void Attack(Vector2 direction);
}
