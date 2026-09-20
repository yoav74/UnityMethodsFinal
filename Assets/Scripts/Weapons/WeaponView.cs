using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <b>View</b> for the active weapon (and later mount): shows the equipped weapon's name and
/// re-renders when it changes, subscribing to <see cref="WeaponInventory.CurrentChanged"/> so it
/// never polls. The inventory lives on the player; the HUD finds it by tag at start rather than
/// holding a serialized cross-object reference, so the HUD can be a scene-independent prefab (no
/// per-scene wiring back to that scene's player).
/// </summary>
public class WeaponView : MonoBehaviour
{
    [SerializeField] private Text label;
    [SerializeField] private string playerTag = "Player";

    private WeaponInventory _inventory;

    private void Start()
    {
        var player = GameObject.FindWithTag(playerTag);
        _inventory = player != null ? player.GetComponent<WeaponInventory>() : null;
        if (_inventory == null)
            return;

        _inventory.CurrentChanged += Render;
        Render(_inventory.Current);
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.CurrentChanged -= Render;
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
