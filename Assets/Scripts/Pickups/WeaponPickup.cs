using UnityEngine;

/// <summary>
/// A world pickup that grants a weapon on contact: it spawns a fresh instance of the weapon
/// prefab under the player and adds it to the <see cref="WeaponInventory"/>, then removes
/// itself (handled by the base <see cref="Pickup"/>). Reused for the hammer, the boomerang and
/// weapons dropped from eggs — only the assigned prefab differs (Open/Closed).
/// </summary>
public class WeaponPickup : Pickup
{
    [SerializeField] private GameObject weaponPrefab;

    protected override void OnCollected(GameObject player)
    {
        var inventory = player.GetComponent<WeaponInventory>();
        if (inventory == null || weaponPrefab == null)
            return;

        GameObject weaponObject = Instantiate(weaponPrefab, player.transform);
        weaponObject.transform.localPosition = Vector3.zero;

        var weapon = weaponObject.GetComponent<IWeapon>();
        if (weapon != null)
            inventory.Add(weapon);
    }
}
