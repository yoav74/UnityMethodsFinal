using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base rideable animal. Provides the <b>shared attack rules</b> (ME-50) as its default attack —
/// a short reach in front that damages enemies (via <see cref="IDamageable"/>, so the ghost
/// shrugs it off) and shatters rocks (via <see cref="IDestructible"/>) but never bonfires (those
/// are <see cref="IFairyDestructible"/>). The three specific animals extend this and override
/// <see cref="Attack"/> for their own style — a tail swipe, a fire spit, a spin (ME-46/47/48).
/// </summary>
public class Animal : MonoBehaviour, IMount
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

    public virtual void Attack(Vector2 direction)
    {
        HitInFront(direction);
    }

    /// <summary>The shared melee reach used by tail/spin animals: enemies + rocks in front.</summary>
    protected void HitInFront(Vector2 direction)
    {
        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * (attackReach * 0.5f);

        Physics2D.OverlapCircle(center, attackReach * 0.5f, TriggerFilter, _hits);
        foreach (var hit in _hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(1); // enemies (ghost immune)
            hit.GetComponent<IDestructible>()?.Hit();        // rocks (not bonfires)
        }
    }
}
