using UnityEngine;

/// <summary>
/// A weapon the player can attack with. The player's attack routes to the current weapon
/// (or a mount) through this abstraction, so new weapons plug in without changing the caller
/// (OCP/DIP). Name + Icon feed the HUD.
/// </summary>
public interface IWeapon
{
    string DisplayName { get; }
    Sprite Icon { get; }

    /// <summary>Attack in the given aim direction (e.g. the way the player faces).</summary>
    void Attack(Vector2 direction);
}
