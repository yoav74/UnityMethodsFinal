using System;

/// <summary>
/// Tracks how many fruits the player has collected across the run. Project-scoped state (it
/// persists across levels, like lives), announcing changes so a HUD can show it later (ME-68).
/// Turning a count of fruits into an extra life is a separate concern handled in ME-52.
/// </summary>
public class FruitCounter
{
    public int Count { get; private set; }

    /// <summary>Raised whenever the count changes, with the new total.</summary>
    public event Action<int> CountChanged;

    /// <summary>Record one collected fruit.</summary>
    public void Add()
    {
        Count++;
        CountChanged?.Invoke(Count);
    }
}
