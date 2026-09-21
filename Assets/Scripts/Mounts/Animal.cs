using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base rideable animal. It owns the <b>shared attack rule</b> (ME-50) in one place —
/// <see cref="ApplyHit"/> — which damages enemies (via <see cref="IDamageable"/>, so the ghost
/// shrugs them off) and shatters rocks (via <see cref="IDestructible"/>) but never bonfires (those
/// are <see cref="IFairyDestructible"/>). Each concrete animal computes its <b>own strike shape</b>
/// (a front reach, a spin, a projectile) and routes it through that rule.
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
    private Vector3 _baseScale = Vector3.one;
    private Vector3 _basePos;
    private Quaternion _baseRot = Quaternion.identity;
    private Coroutine _feedback;

    protected float FeedbackSeconds => attackFeedbackSeconds;

    protected virtual void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Capture the resting transform AFTER the pickup has parented + zeroed us, so the attack
        // feedback (scale/lunge/spin) restores to the right place.
        _baseScale = transform.localScale;
        _basePos = transform.localPosition;
        _baseRot = transform.localRotation;

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

    /// <summary>
    /// The shared animal-attack rule: everything overlapping the circle takes a hit — damage enemies
    /// (the ghost is immune, being non-<see cref="IDamageable"/>) and shatter rocks
    /// (<see cref="IDestructible"/>), never bonfires. Each animal computes its own strike shape (a
    /// front reach, a spin) and routes it through here, so "what a hit does" lives in one place.
    /// </summary>
    protected void ApplyHit(Vector2 center, float radius)
    {
        Physics2D.OverlapCircle(center, radius, TriggerFilter, _hits);
        foreach (var hit in _hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(1);
            hit.GetComponent<IDestructible>()?.Hit();
        }
    }

    // Visible tell that an attack happened. The *motion* is what distinguishes the animals on screen:
    // the base does a lunge scale-punch (used by the fire-spitter, whose projectile is the real tell),
    // while the melee and spin animals override <see cref="AttackFeedback"/> with their own move.
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
        _feedback = StartCoroutine(RunFeedback(dir));
    }

    // Runs this animal's feedback move, then snaps the transform back to rest so overlapping attacks
    // never accumulate an offset/rotation.
    private IEnumerator RunFeedback(Vector2 dir)
    {
        yield return AttackFeedback(dir);
        transform.localScale = _baseScale;
        transform.localPosition = _basePos;
        transform.localRotation = _baseRot;
        _feedback = null;
    }

    /// <summary>+1 facing right, -1 facing left — for aiming a feedback move.</summary>
    protected float FacingSign()
    {
        return _facing != null && !_facing.FacingRight ? -1f : 1f;
    }

    /// <summary>The resting local scale, so subclasses can scale relative to it.</summary>
    protected Vector3 BaseScale => _baseScale;

    /// <summary>
    /// The default attack move: a quick scale "punch" (used by the fire animal). Blue overrides it
    /// with a forward lunge, green with a spin. It is a scale/motion tween (not a colour flash) so it
    /// never fights the fairy's invincibility tint, which drives colour.
    /// </summary>
    protected virtual IEnumerator AttackFeedback(Vector2 dir)
    {
        float t = 0f;
        while (t < attackFeedbackSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / attackFeedbackSeconds)); // 0 -> 1 -> 0
            transform.localScale = _baseScale * (1f + (attackLungeScale - 1f) * k);
            yield return null;
        }
    }
}
