using UnityEngine;

/// <summary>
/// Blue animal mount: its attack is a melee <b>tail swipe</b> — a short reach in front, which is the
/// base <see cref="Animal.HitInFront"/> rule (damages enemies, shatters rocks, spares bonfires). It
/// states that explicitly rather than leaning on an inherited default, so its behaviour is visible
/// at a glance and the base can stay abstract. Collected via the heart pickup (ME-49).
/// </summary>
public class BlueAnimal : Animal
{
    public override void Attack(Vector2 direction)
    {
        HitInFront(direction);
    }
}
