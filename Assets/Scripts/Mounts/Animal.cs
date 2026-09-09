using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base rideable animal. It owns the <b>shared attack rules</b> (ME-50) as reusable helpers —
/// <see cref="HitInFront"/>, <see cref="HitAround"/> — that damage enemies (via
/// <see cref="IDamageable"/>, so the ghost shrugs them off) and shatter rocks (via
/// <see cref="IDestructible"/>) but never bonfires (those are <see cref="IFairyDestructible"/>).
///
/// <see cref="Attack"/> is a <b>Template Method</b>: the base fixes the invariant part — always play
/// the attack feedback — and defers the variable part to <see cref="PerformAttack"/>, which each
/// concrete animal overrides for its own style (a tail swipe, a fire spit, a spin — ME-46/47/48). It
/// is abstract so there is no "default animal" to instantiate by accident (Open/Closed).
///
/// While ridden it also keeps its look consistent with the player: it faces the same way (mirrors
/// the player's <see cref="IFacing"/>) and sits just behind the rider (sorting), so the player isn't
/// hidden under the mount.
/// </summary>
public abstract class Animal : MonoBehaviour, IMount
{
    [SerializeField] private string displayName = "Animal";
    [SerializeField] private Sprite icon;
    [SerializeField] protected float attackReach = 1.5f;

    [Header("Ride visuals")]
    [Tooltip("Where the mount sorts relative to the player sprite (0 = the player's slot; the player body is hidden while mounted).")]
    [SerializeField] private int sortingOffsetFromPlayer = 0;

    [Header("Attack feedback")]
    [Tooltip("Optional effect spawned at the strike point on every attack (a swipe/flash/particle).")]
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private float attackLungeScale = 1.25f;
    [SerializeField] private float attackFeedbackSeconds = 0.12f;

    // Enemies, rocks and hazards use trigger colliders, so the reach query must include triggers
    // regardless of the project-wide queriesHitTriggers setting.
    private static readonly ContactFilter2D TriggerFilter = new ContactFilter2D { useTriggers = true, useLayerMask = false };
    private readonly List<Collider2D> _hits = new List<Collider2D>();

    private SpriteRenderer _sprite;
    private IFacing _facing;
    private Vector3 _baseScale;
    private Coroutine _feedback;

    public string DisplayName => displayName;
    public Sprite Icon => icon;

    protected virtual void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _baseScale = transform.localScale;
    }

    private void Start()
    {
        // Once spawned under the player, mirror its facing and sit just behind the rider.
        _facing = GetComponentInParent<IFacing>();
        if (_facing != null)
        {
            _facing.FacingChanged += OnFacingChanged;
            OnFacingChanged(_facing.FacingRight);

            var playerSprite = (_facing as Component)?.GetComponent<SpriteRenderer>();
            if (_sprite != null && playerSprite != null)
            {
                _sprite.sortingLayerID = playerSprite.sortingLayerID;
                _sprite.sortingOrder = playerSprite.sortingOrder + sortingOffsetFromPlayer;
            }
        }
    }

    private void OnDestroy()
    {
        if (_facing != null)
            _facing.FacingChanged -= OnFacingChanged;
    }

    private void OnFacingChanged(bool facingRight)
    {
        if (_sprite != null)
            _sprite.flipX = !facingRight; // art faces right by default
    }

    /// <summary>
    /// Template Method: run this animal's specific attack, then always show the shared feedback so
    /// every attack reads on screen. Subclasses customise <see cref="PerformAttack"/>, not this.
    /// </summary>
    public void Attack(Vector2 direction)
    {
        PerformAttack(direction);
        PlayAttackFeedback(direction);
    }

    /// <summary>Each animal's own attack, built from the shared hit helpers below.</summary>
    protected abstract void PerformAttack(Vector2 direction);

    /// <summary>The shared melee reach in the facing direction (a tail swipe).</summary>
    protected void HitInFront(Vector2 direction)
    {
        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * (attackReach * 0.5f);
        HitCircle(center, attackReach * 0.5f);
    }

    /// <summary>A hit in a full circle around the animal (a spin attack).</summary>
    protected void HitAround(float radius)
    {
        HitCircle(transform.position, radius);
    }

    // The shared rules applied to everything overlapping a circle: damage enemies (ghost immune),
    // shatter rocks — never bonfires.
    private void HitCircle(Vector2 center, float radius)
    {
        Physics2D.OverlapCircle(center, radius, TriggerFilter, _hits);
        foreach (var hit in _hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(1);
            hit.GetComponent<IDestructible>()?.Hit();
        }
    }

    // Visible tell that an attack happened: a short "lunge" scale-punch on the mount plus, if set,
    // a spawned effect at the strike point. It is a scale punch (not a colour flash) so it never
    // fights the fairy's invincibility tint, which drives colour.
    private void PlayAttackFeedback(Vector2 direction)
    {
        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : (Vector2)(FacingSign() * Vector2.right);

        if (attackEffectPrefab != null)
        {
            Vector3 at = transform.position + (Vector3)(dir * (attackReach * 0.5f));
            Instantiate(attackEffectPrefab, at, Quaternion.identity);
        }

        if (!isActiveAndEnabled)
            return;

        if (_feedback != null)
            StopCoroutine(_feedback);
        _feedback = StartCoroutine(LungePunch());
    }

    private float FacingSign()
    {
        return _facing != null && !_facing.FacingRight ? -1f : 1f;
    }

    private IEnumerator LungePunch()
    {
        float t = 0f;
        while (t < attackFeedbackSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / attackFeedbackSeconds)); // 0 -> 1 -> 0
            transform.localScale = _baseScale * (1f + (attackLungeScale - 1f) * k);
            yield return null;
        }
        transform.localScale = _baseScale;
        _feedback = null;
    }
}
