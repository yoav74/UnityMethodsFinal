/// <summary>
/// Something a projectile or a mount's attack can destroy on contact (a rock, later an enemy).
/// Keeps the projectile from knowing concrete types — it just asks whatever it hit to take a
/// hit (Dependency Inversion). What "hit" means is up to the target.
/// </summary>
public interface IDestructible
{
    void Hit();
}
