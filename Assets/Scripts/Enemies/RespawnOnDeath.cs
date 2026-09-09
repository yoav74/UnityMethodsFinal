using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Respawns its <see cref="Enemy"/> a set time after it dies (the brief: a killed enemy
/// reappears at its original spot). Uses an <b>async Task</b> countdown (Async &amp; Tasks): on
/// <see cref="Enemy.Died"/> it awaits <see cref="Task.Delay(TimeSpan, CancellationToken)"/>, then
/// moves the enemy back to its captured spawn point and reactivates it (which resets its health).
/// The token is cancelled in <see cref="OnDestroy"/> — i.e. on level end — so no respawn fires
/// after teardown; deactivation on death does not cancel it, which is what lets the enemy come
/// back. Await continuations resume on Unity's main thread, so touching the transform is safe.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class RespawnOnDeath : MonoBehaviour
{
    [SerializeField] private float respawnSeconds = 3f;

    private Enemy _enemy;
    private Vector3 _spawnPoint;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _spawnPoint = transform.position;
        _cts = new CancellationTokenSource();
        _enemy.Died += OnDied;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
            _enemy.Died -= OnDied;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private void OnDied(Enemy enemy)
    {
        // Respect the Inspector checkbox: a designer can switch respawning off per enemy. We can't
        // subscribe in OnEnable/unsubscribe in OnDisable, because dying deactivates the GameObject
        // (which would fire OnDisable and cancel the very respawn we want). Awake runs even on a
        // disabled component, so instead we check `enabled` here — false only when the user unticked
        // it, since a normal death leaves `enabled` true while the GameObject goes inactive.
        if (!enabled)
            return;

        _ = RespawnAfterDelay(_cts.Token);
    }

    private async Task RespawnAfterDelay(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(respawnSeconds), token);
            transform.position = _spawnPoint;
            gameObject.SetActive(true); // reactivation resets health via Enemy.OnEnable
        }
        catch (OperationCanceledException)
        {
            // Expected on level end; do not respawn.
        }
    }
}
