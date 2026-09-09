using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base rideable animal. It owns the <b>shared attack rules</b> (ME-50) as reusable helpers —
/// <see cref="HitInFront"/>, <see cref="HitAround"/> — that damage enemies (via
/// <see cref="IDamageable"/>, so the ghost shrugs them off) and shatter rocks (via
/// <see cref="IDestructible"/>) but never bonfires (those are <see cref="IFairyDestructible"/>). It
/// is <b>abstract</b> and leaves <see cref="Attack"/> unimplemented on purpose: there is no
/// "default animal", so every concrete animal must declare its own style — a tail swipe, a fire
/// spit, a spin (ME-46/47/48) — rather than silently inheriting one (Open/Closed, and no base you
/// can instantiate by accident).
/// </summary>
public abstract class Animal : MonoBehaviour, IMount
{
    [SerializeField] private string displayName = "Animal";
    [SerializeField] private Sprite icon;
    [SerializeField] protected float attackReach = 1.5f;

    // Enemies, rocks and hazards use trigger colliders, so the reach query must include triggers
    // regardless of the project-wide queriesHitTriggers setting.
    private static readonly ContactFilter2D TriggerFilter = new ContactFilter2D { useTriggers = true, useLayerMask = false };
    private readonly List<Collider2D> _hits = new List<Collider2D>();

    public string DisplayName => displayName;
    public Sprite Icon => icon;

    /// <summary>Each animal defines its own attack, built from the shared hit helpers below.</summary>
    public abstract void Attack(Vector2 direction);

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
}
