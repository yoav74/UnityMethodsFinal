using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

/// <summary>
/// The <b>scene (level) composition root</b>: binds the systems that should be recreated fresh
/// each time a level loads. Cross-level state (config, lives, level flow, death) is bound one
/// level up in <see cref="ProjectInstaller"/>; scene consumers resolve those from the parent
/// container. New per-level services get a binding here — this is the only place that knows how
/// the level's object graph is composed (Dependency Inversion).
/// </summary>
public class GameInstaller : MonoInstaller
{
    [SerializeField] private InputActionAsset inputActions;

    public override void InstallBindings()
    {
        // Input: bound by its interfaces so consumers see IInputService, while Zenject also
        // drives its IInitializable/IDisposable lifecycle (enable/disable the action map).
        Container.BindInterfacesTo<InputService>().AsSingle().WithArguments(inputActions);

        // Power meter (Async & Tasks): model + the async drain controller. Per level, so it
        // starts full again after a respawn (the level scene is reloaded on death).
        Container.Bind<PowerModel>().AsSingle();
        Container.BindInterfacesTo<PowerController>().AsSingle();
    }
}
