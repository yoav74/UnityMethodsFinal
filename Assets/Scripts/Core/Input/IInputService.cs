/// <summary>
/// The only way gameplay code learns about player input. Consumers depend on this
/// abstraction rather than on <c>Keyboard.current</c> or the Input System directly, so the
/// input backend can change (or be faked in a test) without touching the player, and so
/// nothing has to know which physical key does what (Dependency Inversion).
/// </summary>
public interface IInputService
{
    /// <summary>Horizontal movement, -1 (left) to 1 (right).</summary>
    float MoveAxis { get; }

    /// <summary>True on the frame the jump button went down.</summary>
    bool JumpPressed { get; }

    /// <summary>True on the frame the attack button went down.</summary>
    bool AttackPressed { get; }
}
