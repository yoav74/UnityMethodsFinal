using UnityEngine;
using Zenject;

/// <summary>
/// Start-up check for the composition root. It receives its dependency purely through
/// injection — no <c>Find</c>, no singleton, no serialized reference — so if this logs the
/// configured values, the DI graph is wired correctly.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    private GameConfig _config;

    [Inject]
    public void Construct(GameConfig config)
    {
        _config = config;
    }

    private void Start()
    {
        Debug.Log($"Composition root ready — lives: {_config.StartingLives}, " +
                  $"power: {_config.StartingPower}, fruits per extra life: {_config.FruitsPerExtraLife}");
    }
}
