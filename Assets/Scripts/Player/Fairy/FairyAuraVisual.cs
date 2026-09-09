using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// The <b>visible</b> tell that the fairy's invincibility is on: while it lasts every sprite on the
/// player pulses a tint, so a player — or anyone watching the showcase — can see at a glance that
/// they're currently untouchable. It tints <b>all</b> child sprites, not just the body, so the pulse
/// keeps showing while riding a mount (the mount is a child sprite), and it refreshes that set when
/// the mount changes so a freshly-summoned animal joins the glow and a dismounted one is dropped.
///
/// It is purely cosmetic and owns no rules; it just listens to the shared <see cref="IInvincibility"/>
/// singleton's <see cref="IInvincibility.Changed"/> event (the same instance the fairy pickup
/// activates and the <see cref="FairyAura"/> reads), so the visual and the gameplay effect always
/// agree without either knowing about the other (Single Responsibility, Dependency Inversion).
/// </summary>
public class FairyAuraVisual : MonoBehaviour
{
    [Tooltip("Optional glow/particle child to switch on while invincible. Leave empty to only tint the sprites.")]
    [SerializeField] private GameObject auraEffect;

    [Tooltip("Tint the sprites pulse toward while invincible.")]
    [SerializeField] private Color invincibleTint = new Color(1f, 0.85f, 0.2f);

    [SerializeField] private float pulsesPerSecond = 6f;

    private IInvincibility _invincibility;
    private PlayerMount _mount;

    // Each sprite we tint, remembered with the colour it had before we touched it, so we can restore.
    private readonly Dictionary<SpriteRenderer, Color> _baseColors = new Dictionary<SpriteRenderer, Color>();
    private bool _active;

    [Inject]
    public void Construct(IInvincibility invincibility)
    {
        _invincibility = invincibility;
        _invincibility.Changed += OnChanged;
    }

    private void Awake()
    {
        // The mount lives as a child of the player; refresh the tinted set whenever it changes.
        _mount = GetComponentInChildren<PlayerMount>();
        if (_mount == null)
            _mount = GetComponent<PlayerMount>();
        if (_mount != null)
            _mount.MountChanged += OnMountChanged;
    }

    private void Start()
    {
        // Injection has run by now; sync with whatever the state already is.
        if (_invincibility != null)
            OnChanged(_invincibility.IsActive);
    }

    private void OnDestroy()
    {
        if (_invincibility != null)
            _invincibility.Changed -= OnChanged;
        if (_mount != null)
            _mount.MountChanged -= OnMountChanged;
    }

    private void OnMountChanged(IMount _)
    {
        if (_active)
            RefreshSprites(); // pull the new mount into the glow (or drop the old one)
    }

    private void OnChanged(bool active)
    {
        _active = active;

        if (auraEffect != null)
            auraEffect.SetActive(active);

        if (active)
            RefreshSprites();
        else
            RestoreAndClear();
    }

    // Rebuild the tinted set from the current child sprites, keeping known base colours and capturing
    // any new sprite's current (untinted) colour as its base.
    private void RefreshSprites()
    {
        var current = GetComponentsInChildren<SpriteRenderer>(true);
        var set = new HashSet<SpriteRenderer>(current);

        // Drop entries whose sprite is gone (e.g. a dismounted animal was destroyed).
        var stale = new List<SpriteRenderer>();
        foreach (var kv in _baseColors)
            if (kv.Key == null || !set.Contains(kv.Key))
                stale.Add(kv.Key);
        foreach (var s in stale)
            _baseColors.Remove(s);

        foreach (var sr in current)
            if (sr != null && !_baseColors.ContainsKey(sr))
                _baseColors[sr] = sr.color; // capture before we start tinting it
    }

    private void RestoreAndClear()
    {
        foreach (var kv in _baseColors)
            if (kv.Key != null)
                kv.Key.color = kv.Value;
        _baseColors.Clear();
    }

    private void Update()
    {
        if (!_active)
            return;

        float t = Mathf.PingPong(Time.time * pulsesPerSecond, 1f);
        foreach (var kv in _baseColors)
            if (kv.Key != null)
                kv.Key.color = Color.Lerp(kv.Value, invincibleTint, t);
    }
}
