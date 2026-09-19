using System.Collections;
using UnityEngine;

/// <summary>
/// Green animal mount: its attack is a <b>spin</b> in place — a full circle around itself
/// (<see cref="Animal.HitAround"/>) so it strikes on all sides, following the shared animal rules
/// (damages enemies, shatters rocks, spares bonfires). On screen it <b>rotates a full turn</b>, which
/// reads clearly as a spin and distinct from the blue animal's forward swipe. Collected via the star
/// pickup (ME-49).
/// </summary>
public class GreenAnimal : Animal
{
    [SerializeField] private float spinRadius = 1.5f;
    [SerializeField] private float spinSeconds = 0.35f;

    protected override void PerformAttack(Vector2 direction)
    {
        HitAround(spinRadius);
    }

    protected override IEnumerator AttackFeedback(Vector2 dir)
    {
        Quaternion baseRot = transform.localRotation;
        float t = 0f;
        while (t < spinSeconds)
        {
            t += Time.deltaTime;
            float k = t / spinSeconds;
            transform.localRotation = baseRot * Quaternion.Euler(0f, 0f, -360f * k * FacingSign());
            yield return null;
        }
    }
}
