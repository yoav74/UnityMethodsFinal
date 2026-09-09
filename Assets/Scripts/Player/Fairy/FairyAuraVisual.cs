using UnityEngine;
using Zenject;

/// <summary>
/// The <b>visible</b> tell that the fairy's invincibility is on: while it lasts the player pulses a
/// tint (and, if assigned, a glow/particle child turns on), so a player — or anyone watching the
/// showcase — can see at a glance that they're currently untouchable. It is purely cosmetic and owns
/// no rules; it just listens to the shared <see cref="IInvincibility"/> singleton's
/// <see cref="IInvincibility.Changed"/> event (the same instance the fairy pickup activates and the
/// <see cref="FairyAura"/> reads), so the visual and the gameplay effect always agree without either
/// knowing about the other (Single Responsibility, Dependency Inversion).
/// </summary>
public class FairyAuraVisual : MonoBehaviour
{
    [Tooltip("Optional glow/particle child to switch on while invincible. Leave empty to only tint the sprite.")]
    [SerializeField] private GameObject auraEffect;

    [Tooltip("Tint applied to the player sprite while invincible; it pulses toward the base colour.")]
    [SerializeField] private Color invincibleTint = new Color(1f, 0.85f, 0.2f);

    [SerializeField] private float pulsesPerSecond = 6f;

    private IInvincibility _invincibility;
    private SpriteRenderer _sprite;
    private Color _baseColor = Color.white;
    private bool _active;

    [Inject]
    public void Construct(IInvincibility invincibility)
    {
        _invincibility = invincibility;
        _invincibility.Changed += OnChanged;
        OnChanged(_invincibility.IsActive); // sync with whatever the state already is
    }

    private void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        if (_sprite != null)
            _baseColor = _sprite.color;
    }

    private void OnDestroy()
    {
        if (_invincibility != null)
            _invincibility.Changed -= OnChanged;
    }

    private void OnChanged(bool active)
    {
        _active = active;

        if (auraEffect != null)
            auraEffect.SetActive(active);

        if (_sprite != null && !active)
            _sprite.color = _baseColor; // restore when the window ends
    }

    private void Update()
    {
        if (!_active || _sprite == null)
            return;

        // Pulse between the base colour and the invincible tint so the effect reads as "flashing".
        float t = Mathf.PingPong(Time.time * pulsesPerSecond, 1f);
        _sprite.color = Color.Lerp(_baseColor, invincibleTint, t);
    }
}
