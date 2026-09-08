/// <summary>
/// Handles a player death: spend a life, then restart the current level so the player respawns
/// at its start. If this was the last life, the death is fatal — <see cref="LivesModel.LoseLife"/>
/// raises <c>GameOver</c> and <see cref="LivesController"/> owns the reset-and-restart, so this
/// service deliberately does nothing further in that case. Whether the death was fatal is
/// captured <i>before</i> spending the life, so the two paths never both fire (no re-entrancy race).
/// </summary>
public class DeathService : IDeathService
{
    private readonly LivesModel _lives;
    private readonly ILevelLoader _levels;

    public DeathService(LivesModel lives, ILevelLoader levels)
    {
        _lives = lives;
        _levels = levels;
    }

    public void Die()
    {
        bool fatal = _lives.Lives <= 1;
        _lives.LoseLife();

        if (!fatal)
            _levels.ReloadCurrent();
        // fatal: GameOver was raised; LivesController resets lives and loads the first level.
    }
}
