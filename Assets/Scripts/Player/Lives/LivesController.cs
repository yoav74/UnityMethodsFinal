using UnityEngine;
using Zenject;

/// <summary>
/// <b>Controller</b> for lives: reacts to model events and drives the game-level response.
/// It owns the <c>GameOver</c> path — reset the lives to full and restart from the first level.
/// The non-fatal path (a life lost while lives remain) is owned by <see cref="DeathService"/>,
/// so exactly one of them acts per death. Built by the container as an <see cref="IInitializable"/>,
/// so it needs no scene object and lives at project scope alongside the model.
/// </summary>
public class LivesController : IInitializable, System.IDisposable
{
    private readonly LivesModel _model;
    private readonly ILevelLoader _levels;

    public LivesController(LivesModel model, ILevelLoader levels)
    {
        _model = model;
        _levels = levels;
    }

    public void Initialize()
    {
        _model.GameOver += OnGameOver;
    }

    public void Dispose()
    {
        _model.GameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        Debug.Log("Game over - resetting to the first level.");
        _model.ResetToStart();
        _levels.LoadFirst();
    }
}
