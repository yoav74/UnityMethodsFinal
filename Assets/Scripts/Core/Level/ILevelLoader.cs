/// <summary>
/// Abstraction over loading levels, so gameplay flow (death, game over, level end) depends on an
/// intent — "reload this level", "go to the first level", "advance to the next" — not on the scene
/// API directly (Dependency Inversion).
/// </summary>
public interface ILevelLoader
{
    /// <summary>Restart the current level from the beginning (a death with lives remaining).</summary>
    void ReloadCurrent();

    /// <summary>Load the first level (a fresh game, on game over).</summary>
    void LoadFirst();

    /// <summary>Advance to the next level (the level was completed); wraps to the first past the end.</summary>
    void LoadNext();
}
