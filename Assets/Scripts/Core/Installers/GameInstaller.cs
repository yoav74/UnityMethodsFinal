using UnityEngine;
using Zenject;

/// <summary>
/// The game's <b>composition root</b>: the single place where concrete implementations are
/// bound to the abstractions the rest of the game depends on. Every other system receives
/// what it needs through injection instead of looking it up (no <c>Find</c>, no singletons),
/// which keeps dependencies explicit and swappable (Dependency Inversion).
///
/// New services get a binding here — this is the only file that knows how the object graph
/// is composed.
/// </summary>
public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig gameConfig;

    public override void InstallBindings()
    {
        // Shared, read-only tuning data for the whole scene.
        Container.BindInstance(gameConfig).AsSingle();
    }
}
