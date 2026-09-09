using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// <see cref="IInvincibility"/> timed with an <b>async Task</b> (Async &amp; Tasks): <see cref="Activate"/>
/// turns invincibility on immediately and awaits <see cref="Task.Delay(TimeSpan, CancellationToken)"/>
/// for <see cref="GameConfig.FairyInvincibilitySeconds"/> before turning it off. Re-activating
/// cancels the previous window and starts fresh, and <see cref="Dispose"/> cancels on level end /
/// container teardown so nothing fires afterwards. A plain C# service built by the container;
/// await continuations resume on Unity's main thread, so the state flips there.
/// </summary>
public class Invincibility : IInvincibility, IDisposable
{
    private readonly float _seconds;
    private CancellationTokenSource _cts;

    public bool IsActive { get; private set; }
    public event Action<bool> Changed;

    public Invincibility(GameConfig config)
    {
        _seconds = config.FairyInvincibilitySeconds;
    }

    public void Activate()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        SetActive(true);
        _ = Countdown(_cts.Token);
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task Countdown(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(_seconds), token);
            SetActive(false);
        }
        catch (OperationCanceledException)
        {
            // Re-activated or torn down; leave the state to whoever cancelled us.
        }
    }

    private void SetActive(bool value)
    {
        IsActive = value;
        Changed?.Invoke(value);
    }
}
