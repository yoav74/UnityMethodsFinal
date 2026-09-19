using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// <b>View</b> for the power meter, drawn as a row of bars — one per point of
/// <see cref="PowerModel.Max"/> — like the reference game's energy gauge. It builds the bars once
/// from a sprite, then on every <see cref="PowerModel.PowerChanged"/> lights the first
/// <c>Power</c> of them and dims the rest. It only reads the injected model and writes to UI — no
/// game logic here.
/// </summary>
public class PowerView : MonoBehaviour
{
    [SerializeField] private Sprite barSprite;              // Sprite_Power_Line
    [SerializeField] private RectTransform container;       // where bars go; defaults to this object
    [SerializeField] private Vector2 barSize = new Vector2(10f, 22f);
    [SerializeField] private float spacing = 3f;
    [SerializeField] private Color filledColor = new Color(1f, 0.82f, 0.15f);
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.15f);

    private PowerModel _model;
    private readonly List<Image> _bars = new List<Image>();

    [Inject]
    public void Construct(PowerModel model)
    {
        _model = model;
    }

    private void Start()
    {
        if (_model == null)
            return;

        if (container == null)
            container = transform as RectTransform;

        // If this object still carries the old text label, clear it so it doesn't show behind bars.
        var text = GetComponent<Text>();
        if (text != null)
            text.text = string.Empty;

        BuildBars(_model.Max);
        _model.PowerChanged += Render;
        Render(_model.Power);
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.PowerChanged -= Render;
    }

    private void BuildBars(int count)
    {
        // Lay the row out centred on the container's midpoint, so placing the container at the top
        // centre of the screen centres the whole meter.
        float step = barSize.x + spacing;
        float rowWidth = count * barSize.x + (count - 1) * spacing;
        float startX = -rowWidth * 0.5f + barSize.x * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject($"Bar{i}", typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(container, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = barSize;
            rt.anchoredPosition = new Vector2(startX + i * step, 0f);

            var img = go.GetComponent<Image>();
            img.sprite = barSprite;
            img.raycastTarget = false;
            _bars.Add(img);
        }
    }

    private void Render(int power)
    {
        for (int i = 0; i < _bars.Count; i++)
            _bars[i].color = i < power ? filledColor : emptyColor;
    }
}
