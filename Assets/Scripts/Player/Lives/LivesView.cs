using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// <b>View</b> for the lives model: renders the count and re-renders on change. It only
/// reads the injected model and writes to a UI label (falling back to a log until the HUD is
/// built in ME-68) — no game logic lives here.
/// </summary>
public class LivesView : MonoBehaviour
{
    [SerializeField] private Text label;

    private LivesModel _model;

    [Inject]
    public void Construct(LivesModel model)
    {
        _model = model;
    }

    private void Start()
    {
        if (_model == null)
            return;

        _model.LivesChanged += Render;
        Render(_model.Lives);
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.LivesChanged -= Render;
    }

    private void Render(int lives)
    {
        if (label != null)
            label.text = $"Lives: {lives}";
        else
            Debug.Log($"[HUD] Lives: {lives}");
    }
}
