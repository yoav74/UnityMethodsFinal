/// <summary>
/// Abstraction over loading levels, so gameplay flow (death, game over, level end) depends on
/// an intent — "reload this level", "go to the first level" — not on the scene API directly.
/// </summary>
public interface ILevelLoader
{
    /// <summary>Restart the current level from the beginning.</summary>
    void ReloadCurrent();

    /// <summary>Load the first level (a fresh game).</summary>
    void LoadFirst();
}
