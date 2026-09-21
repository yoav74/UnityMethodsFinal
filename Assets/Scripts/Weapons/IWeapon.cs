/// <summary>
/// A weapon the player can attack with, plus the name the HUD shows. It extends
/// <see cref="IAttacker"/> — so the player's attack treats weapons and mounts alike — and adds only
/// <see cref="DisplayName"/>, which is the one extra member a client actually reads (WeaponView).
/// New weapons plug in without changing the caller (OCP/DIP).
/// </summary>
public interface IWeapon : IAttacker
{
    string DisplayName { get; }
}
