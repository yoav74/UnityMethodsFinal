using System;

/// <summary>
/// Which way the player is currently facing, exposed so things that ride along — a mount, an
/// aimed attack — can turn with the player without depending on the concrete movement component
/// (Dependency Inversion). <see cref="FacingChanged"/> fires only when the direction actually
/// flips, so listeners can mirror it cheaply.
/// </summary>
public interface IFacing
{
    /// <summary>True when facing right (art faces right by default), false when facing left.</summary>
    bool FacingRight { get; }

    /// <summary>Raised when the facing flips; the argument is the new <see cref="FacingRight"/>.</summary>
    event Action<bool> FacingChanged;
}
