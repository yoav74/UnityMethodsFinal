using UnityEngine;
using Zenject;

/// <summary>
/// Turns the attack button into an attack. It routes to the current weapon in the
/// <see cref="WeaponInventory"/>, firing in the direction the player faces. When mounts land
/// (ME-45) this also routes to the mount's attack. Per the brief, with no weapon and no
/// mount the player simply can't attack.
/// </summary>
[RequireComponent(typeof(WeaponInventory))]
public class PlayerAttack : MonoBehaviour
{
    private IInputService _input;
    private WeaponInventory _inventory;
    private PlayerMovement _movement;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Awake()
    {
        _inventory = GetComponent<WeaponInventory>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!_input.AttackPressed)
            return;

        // TODO (ME-45): if riding a mount, the mount attacks instead of the weapon.
        if (_inventory.HasWeapon)
        {
            Vector2 direction = (_movement != null && !_movement.FacingRight) ? Vector2.left : Vector2.right;
            _inventory.Current.Attack(direction);
        }
    }
}
