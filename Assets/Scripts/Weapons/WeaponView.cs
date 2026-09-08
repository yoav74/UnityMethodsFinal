using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <b>View</b> for the active weapon (and later mount): shows the equipped weapon's name and
/// re-renders when it changes, subscribing to <see cref="WeaponInventory.CurrentChanged"/> so it
/// never polls. The inventory is a scene component (on the player), referenced directly.
/// </summary>
public class WeaponView : MonoBehaviour
{
    [SerializeField] private Text label;
    [SerializeField] private WeaponInventory inventory;

    private void Start()
    {
        if (inventory == null)
            return;

        inventory.CurrentChanged += Render;
        Render(inventory.Current);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.CurrentChanged -= Render;
    }

    private void Render(IWeapon weapon)
    {
        string weaponName = weapon != null ? weapon.DisplayName : "none";
        if (label != null)
            label.text = $"Weapon: {weaponName}";
        else
            Debug.Log($"[HUD] Weapon: {weaponName}");
    }
}
