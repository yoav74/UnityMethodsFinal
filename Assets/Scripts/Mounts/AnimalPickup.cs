using UnityEngine;

/// <summary>
/// A collectible that grants a mount (the heart → blue, leaf → red, star → green). On contact it
/// spawns its animal under the player and hands it to the <see cref="PlayerMount"/>, which mounts
/// it — or swaps it in if the player is already riding. Reuses the base <see cref="Pickup"/> for
/// detection + self-removal; only the assigned animal prefab differs (Open/Closed).
/// </summary>
public class AnimalPickup : Pickup
{
    [SerializeField] private GameObject animalPrefab;

    protected override void OnCollected(GameObject player)
    {
        var mount = player.GetComponent<PlayerMount>();
        if (mount == null || animalPrefab == null)
            return;

        GameObject animalObject = Instantiate(animalPrefab, player.transform);
        animalObject.transform.localPosition = Vector3.zero;

        var animal = animalObject.GetComponent<IMount>();
        if (animal != null)
            mount.Mount(animal);
    }
}
