using System;

/// <summary>
/// The player's temporary invincibility (granted by the fairy). Contact-damage sources query
/// <see cref="IsActive"/> to know whether to spare the player, and the fairy aura uses it to
/// decide whether contact destroys what it touches. Kept behind an interface so those callers
/// stay ignorant of how the window is timed (Dependency Inversion).
/// </summary>
public interface IInvincibility
{
    bool IsActive { get; }

    /// <summary>Start (or restart) the invincibility window.</summary>
    void Activate();

    /// <summary>Raised when invincibility turns on (true) or expires (false) — for the HUD.</summary>
    event Action<bool> Changed;
}
