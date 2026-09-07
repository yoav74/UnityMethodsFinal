using UnityEngine;

/// <summary>
/// TEMP: until weapon pickups exist (ME-42), grants the player any weapon components already
/// on it/its children at start, so attacking is testable. Removed once pickups gate weapons.
/// </summary>
[RequireComponent(typeof(WeaponInventory))]
public class PlayerLoadout : MonoBehaviour
{
    private void Start()
    {
        WeaponInventory inventory = GetComponent<WeaponInventory>();
        foreach (IWeapon weapon in GetComponentsInChildren<IWeapon>(true))
            inventory.Add(weapon);
    }
}
