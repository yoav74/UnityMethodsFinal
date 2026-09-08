/// <summary>
/// Something with health that a weapon or a mount's attack can hurt. Projectiles depend on this
/// abstraction rather than on concrete enemy types (Dependency Inversion); how much a hit means,
/// and what dying does, is up to the target. Distinct from <see cref="IDestructible"/>, which is
/// a one-shot destroy for scenery like rocks.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int amount);
}
