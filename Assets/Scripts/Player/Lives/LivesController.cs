using UnityEngine;
using Zenject;

/// <summary>
/// <b>Controller</b> for lives: reacts to model events and drives the game-level response.
/// On game over it resets the lives to start (full level/scene flow arrives with ME-66).
/// Built by the container as an <see cref="IInitializable"/>, so it needs no scene object.
/// </summary>
public class LivesController : IInitializable, System.IDisposable
{
    private readonly LivesModel _model;

    public LivesController(LivesModel model)
    {
        _model = model;
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
        Debug.Log("Game over - resetting lives to start.");
        _model.ResetToStart();
        // TODO (ME-66): reload the first level here.
    }
}
