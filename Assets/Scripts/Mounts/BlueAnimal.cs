using System.Collections;
using UnityEngine;

/// <summary>
/// Blue animal mount: its attack is a melee <b>tail swipe</b> — a short reach in front of the animal,
/// routed through the shared <see cref="Animal.ApplyHit"/> rule (damages enemies, shatters rocks,
/// spares bonfires). On screen it <b>whips in place</b> — a quick rotational lash toward the facing
/// direction and back — so the swipe reads without the animal dashing out of position, and stays
/// clearly different from the green animal's full spin. Collected via the heart pickup (ME-49).
/// </summary>
public class BlueAnimal : Animal
{
    [Tooltip("Peak tilt of the tail-swipe whip, in degrees (flip the sign if it lashes the wrong way).")]
    [SerializeField] private float swipeAngle = 35f;
    [SerializeField] private float swipeSeconds = 0.18f;

    protected override void PerformAttack(Vector2 direction)
    {
        // This animal's own strike shape: a short circle just in front, in the facing direction.
        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * (attackReach * 0.5f);
        ApplyHit(center, attackReach * 0.5f);
    }

    // A quick rotational lash toward the facing direction and back — a tail swipe, not a forward
    // dash: the body pivots in place (0 -> peak -> 0) so it reads as a swipe rather than a lunge.
    protected override IEnumerator AttackFeedback(Vector2 dir)
    {
        Quaternion baseRot = transform.localRotation;
        float sign = FacingSign(); // +1 facing right, -1 facing left
        float t = 0f;
        while (t < swipeSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.PI * (t / swipeSeconds)); // 0 -> 1 -> 0
            transform.localRotation = baseRot * Quaternion.Euler(0f, 0f, -swipeAngle * sign * k);
            yield return null;
        }
        transform.localRotation = baseRot;
    }
}
