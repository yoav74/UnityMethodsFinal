using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// <b>View</b> for the power meter: renders the value and re-renders on change. It only reads
/// the injected model and writes to a UI label (falling back to a log until the HUD is built
/// in ME-68) — no game logic here.
/// </summary>
public class PowerView : MonoBehaviour
{
    [SerializeField] private Text label;

    private PowerModel _model;

    [Inject]
    public void Construct(PowerModel model)
    {
        _model = model;
    }

    private void Start()
    {
        if (_model == null)
            return;

        _model.PowerChanged += Render;
        Render(_model.Power);
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.PowerChanged -= Render;
    }

    private void Render(int power)
    {
        if (label != null)
            label.text = $"Power: {power}/{_model.Max}";
        else
            Debug.Log($"[HUD] Power: {power}/{_model.Max}");
    }
}
