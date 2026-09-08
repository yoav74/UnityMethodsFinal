/// <summary>
/// A pool a projectile returns itself to when spent. Kept minimal (just <see cref="Return"/>)
/// so a <see cref="BaseProjectile"/> depends on the abstraction, not a concrete pool (DIP/ISP).
/// </summary>
public interface IProjectilePool
{
    void Return(BaseProjectile projectile);
}
