using UnityEngine;

/// <summary>
/// Green animal mount: its attack is a <b>spin</b> in place — a full circle around itself rather
/// than a reach in front, so it strikes on all sides. It follows the shared animal rules (damages
/// enemies, shatters rocks, spares bonfires) via the base <see cref="Animal.HitAround"/>. Collected
/// via the star pickup (ME-49).
/// </summary>
public class GreenAnimal : Animal
{
    [SerializeField] private float spinRadius = 1.5f;

    protected override void PerformAttack(Vector2 direction)
    {
        HitAround(spinRadius);
    }
}
