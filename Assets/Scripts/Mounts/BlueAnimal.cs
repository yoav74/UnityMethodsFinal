using System.Collections;
using UnityEngine;

/// <summary>
/// Blue animal mount: its attack is a melee <b>tail swipe</b> — a short reach in front
/// (<see cref="Animal.HitInFront"/>: damages enemies, shatters rocks, spares bonfires), and on screen
/// it <b>lunges forward</b> in the facing direction so the swipe is visible and clearly different from
/// the green animal's spin. Collected via the heart pickup (ME-49).
/// </summary>
public class BlueAnimal : Animal
{
    [SerializeField] private float lungeDistance = 0.5f;
    [SerializeField] private float lungeSeconds = 0.16f;

    protected override void PerformAttack(Vector2 direction)
    {
        HitInFront(direction);
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
