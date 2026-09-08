/// <summary>
/// The single entry point for "the player died". Everything that can kill the player — power
/// hitting zero, a rock draining the last power, a bonfire, an enemy — calls <see cref="Die"/>
/// and stays ignorant of lives, respawns and level flow (Single Responsibility / DIP).
/// </summary>
public interface IDeathService
{
    void Die();
}
