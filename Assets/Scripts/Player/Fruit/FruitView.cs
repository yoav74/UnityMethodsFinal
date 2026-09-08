using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// <b>View</b> for the fruit counter: renders the total and re-renders on change. It only reads
/// the injected model and writes to a UI label (log fallback) — no game logic here.
/// </summary>
public class FruitView : MonoBehaviour
{
    [SerializeField] private Text label;

    private FruitCounter _fruits;

    [Inject]
    public void Construct(FruitCounter fruits)
    {
        _fruits = fruits;
    }

    private void Start()
    {
        if (_fruits == null)
            return;

        _fruits.CountChanged += Render;
        Render(_fruits.Count);
    }

    private void OnDestroy()
    {
        if (_fruits != null)
            _fruits.CountChanged -= Render;
    }

    private void Render(int count)
    {
        if (label != null)
            label.text = $"Fruits: {count}";
        else
            Debug.Log($"[HUD] Fruits: {count}");
    }
}
