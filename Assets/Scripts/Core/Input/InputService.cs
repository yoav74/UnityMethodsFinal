using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

/// <summary>
/// <see cref="IInputService"/> backed by the project's Input System asset. It is a plain C#
/// class (not a MonoBehaviour) built by the container: Zenject calls
/// <see cref="Initialize"/> to enable the Player action map and <see cref="Dispose"/> to
/// disable it, so the map's lifetime matches the container's.
///
/// Single responsibility: translate the Input System into the three signals the game needs.
/// </summary>
public class InputService : IInputService, IInitializable, IDisposable
{
    private const string PlayerMap = "Player";

    private readonly InputActionAsset _actions;

    private InputActionMap _playerMap;
    private InputAction _move;
    private InputAction _jump;
    private InputAction _attack;

    public InputService(InputActionAsset actions)
    {
        _actions = actions;
    }

    public void Initialize()
    {
        if (_actions == null)
        {
            Debug.LogError("InputService: no InputActionAsset was provided to the installer.");
            return;
        }

        _playerMap = _actions.FindActionMap(PlayerMap, throwIfNotFound: true);
        _move = _playerMap.FindAction("Move", throwIfNotFound: true);
        _jump = _playerMap.FindAction("Jump", throwIfNotFound: true);
        _attack = _playerMap.FindAction("Attack", throwIfNotFound: true);

        _playerMap.Enable();
    }

    public void Dispose()
    {
        _playerMap?.Disable();
    }

    public float MoveAxis => _move?.ReadValue<Vector2>().x ?? 0f;

    public bool JumpPressed => _jump != null && _jump.WasPressedThisFrame();

    public bool AttackPressed => _attack != null && _attack.WasPressedThisFrame();
}
