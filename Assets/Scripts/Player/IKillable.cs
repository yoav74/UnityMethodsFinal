/// <summary>
/// Something that can be killed on contact — implemented by the player so hazards and enemy
/// projectiles don't need to know about lives or the death service (Dependency Inversion). The
/// target decides what dying means.
/// </summary>
public interface IKillable
{
    void Kill();
}
