using UnityEngine;
using Zenject;

/// <summary>
/// A fruit the player collects to restore power. Two types exist by tuning the same component:
/// Fruit 1 = +1, Fruit 2 = +2 (no subclass — OCP). On pickup it tops up the power meter and
/// records the fruit toward the collection count; the base <see cref="Pickup"/> removes it.
/// The power meter and counter are injected (the models, not scene objects), so the fruit stays
/// ignorant of how they work.
/// </summary>
public class FruitPickup : Pickup
{
    [SerializeField] private int power = 1;

    private PowerModel _power;
    private FruitCounter _fruits;

    [Inject]
    public void Construct(PowerModel power, FruitCounter fruits)
    {
        _power = power;
        _fruits = fruits;
    }

    protected override void OnCollected(GameObject player)
    {
        _power?.Add(power);
        _fruits?.Add();
    }
}
