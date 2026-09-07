using UnityEngine;
using Zenject;

/// <summary>
/// Start-up check for the composition root. It receives its dependencies purely through
/// injection — no <c>Find</c>, no singleton, no serialized reference — so if this reports the
/// configured values and reacts to input, the DI graph is wired correctly.
///
/// Temporary: the input echo here exists to verify <see cref="IInputService"/> until the
/// player controller consumes it (ME-31/32/33).
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    private GameConfig _config;
    private IInputService _input;

    [Inject]
    public void Construct(GameConfig config, IInputService input)
    {
        _config = config;
        _input = input;
    }

    private void Start()
    {
        Debug.Log($"Composition root ready — lives: {_config.StartingLives}, " +
                  $"power: {_config.StartingPower}, fruits per extra life: {_config.FruitsPerExtraLife}");
        Debug.Log($"Input service resolved: {_input.GetType().Name}");
    }

    private void Update()
    {
        if (_input.JumpPressed)
            Debug.Log("Input: Jump");

        if (_input.AttackPressed)
            Debug.Log("Input: Attack");
    }
}
