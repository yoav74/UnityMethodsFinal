using UnityEngine;
using Zenject;

/// <summary>
/// The <b>project-scoped composition root</b> (lives on the <c>ProjectContext</c> in Resources).
/// Everything bound here is a single instance for the whole run and <i>survives scene reloads</i>,
/// which is exactly what the cross-level state needs:
///
/// <list type="bullet">
///   <item><see cref="GameConfig"/> — shared tuning, needed by both project and scene systems.</item>
///   <item><see cref="LivesModel"/> + <see cref="LivesController"/> — lives persist across a
///   level restart (losing a life and retrying keeps the lives you have left).</item>
///   <item><see cref="ILevelLoader"/> and <see cref="IDeathService"/> — the level-flow and death
///   services the whole game shares.</item>
/// </list>
///
/// Per-level state (input, the power meter) is bound in <see cref="GameInstaller"/> instead, so it
/// is recreated fresh each time a level loads.
/// </summary>
public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameConfig gameConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(gameConfig).AsSingle();

        // Lives (MVC), persistent across levels.
        Container.Bind<LivesModel>().AsSingle();
        Container.Bind<FruitCounter>().AsSingle();
        Container.BindInterfacesTo<LivesController>().AsSingle();

        // Level flow + death, shared by the whole game.
        Container.Bind<ILevelLoader>().To<SceneLevelLoader>().AsSingle();
        Container.Bind<IDeathService>().To<DeathService>().AsSingle();
    }
}
