using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Drives the power meter over time with an <b>async drain loop</b> (Async &amp; Tasks): every
/// <see cref="GameConfig.PowerDrainSeconds"/> it awaits <see cref="Task.Delay(TimeSpan, CancellationToken)"/>
/// and drains one point. The loop is owned by a <see cref="CancellationTokenSource"/> that is
/// cancelled on <see cref="Dispose"/>, so it stops cleanly when the scene/container tears down
/// (no dangling task, no work after teardown).
///
/// When power hits zero the player dies: it costs a life and the meter refills for the respawn
/// (full death/respawn flow is finished in ME-36). Await continuations resume on Unity's main
/// thread via its synchronization context, so touching the model here is safe.
/// </summary>
public class PowerController : IInitializable, IDisposable
{
    private readonly PowerModel _power;
    private readonly LivesModel _lives;
    private readonly float _drainSeconds;

    private CancellationTokenSource _cts;

    public PowerController(PowerModel power, LivesModel lives, GameConfig config)
    {
        _power = power;
        _lives = lives;
        _drainSeconds = config.PowerDrainSeconds;
    }

    public void Initialize()
    {
        _power.PowerDepleted += OnDepleted;
        _cts = new CancellationTokenSource();
        _ = DrainLoop(_cts.Token);
    }

    public void Dispose()
    {
        _power.PowerDepleted -= OnDepleted;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task DrainLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(_drainSeconds), token);
                _power.Drain(1);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected: the container was torn down; end the loop quietly.
        }
    }

    private void OnDepleted()
    {
        _lives.LoseLife();
        _power.Reset();
    }
}
