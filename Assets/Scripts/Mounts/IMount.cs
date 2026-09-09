using UnityEngine;

/// <summary>
/// A rideable animal seen through what the player needs from it: a name/icon for the HUD and an
/// attack. The player's attack and mount slot depend on this abstraction, so the three animals
/// (blue/red/green) plug in without either knowing the concrete type (Open/Closed, DIP).
/// </summary>
public interface IMount
{
    string DisplayName { get; }
    Sprite Icon { get; }
    void Attack(Vector2 direction);
}
