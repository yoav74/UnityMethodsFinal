using System.Collections;
using UnityEngine;

/// <summary>
/// Blue animal mount: its attack is a melee <b>tail swipe</b> — a short reach in front of the animal,
/// routed through the shared <see cref="Animal.ApplyHit"/> rule (damages enemies, shatters rocks,
/// spares bonfires). On screen it lunges forward so the swipe reads, clearly different from the green
/// animal's spin. Collected via the heart pickup (ME-49).
/// </summary>
public class BlueAnimal : Animal
{
    [SerializeField] private float lungeDistance = 0.5f;
    [SerializeField] private float lungeSeconds = 0.16f;

    protected override void PerformAttack(Vector2 direction)
    {
        // This animal's own strike shape: a short circle just in front, in the facing direction.
        Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * (attackReach * 0.5f);
        ApplyHit(center, attackReach * 0.5f);
    }

    protected override IEnumerator AttackFeedback(Vector2 dir)
    {
        Vector3 basePos = transform.localPosition;
        float t = 0f;
        while (t < lungeSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.PI * (t / lungeSeconds)); // 0 -> 1 -> 0
            transform.localPosition = basePos + (Vector3)(dir * (lungeDistance * k));
            yield return null;
        }
    }
}
