using System;

/// <summary>
/// <b>Model</b> of the player's lives. Owns the count and the rules (lose/gain/reset), and
/// announces changes through events — it knows nothing about UI or scenes. Starts at
/// <see cref="GameConfig.StartingLives"/> (brief: 3).
/// </summary>
public class LivesModel
{
    private readonly int _startingLives;

    public int Lives { get; private set; }

    /// <summary>Raised whenever the count changes, with the new value.</summary>
    public event Action<int> LivesChanged;

    /// <summary>Raised the moment lives reach zero.</summary>
    public event Action GameOver;

    public LivesModel(GameConfig config)
    {
        _startingLives = config.StartingLives;
        Lives = _startingLives;
    }

    public void LoseLife()
    {
        if (Lives <= 0)
            return;

        Lives--;
        LivesChanged?.Invoke(Lives);

        if (Lives <= 0)
            GameOver?.Invoke();
    }

    public void GainLife()
    {
        Lives++;
        LivesChanged?.Invoke(Lives);
    }

    /// <summary>Reset to the starting count (a fresh game).</summary>
    public void ResetToStart()
    {
        Lives = _startingLives;
        LivesChanged?.Invoke(Lives);
    }
}
