using UnityEngine;
using Zenject;

/// <summary>
/// Turns the attack button into an attack, firing in the direction the player faces. Riding a
/// mount takes priority — the animal attacks instead of the weapon; otherwise the current weapon
/// in the <see cref="WeaponInventory"/> fires. Per the brief, with no mount and no weapon the
/// player simply can't attack.
/// </summary>
[RequireComponent(typeof(WeaponInventory))]
public class PlayerAttack : MonoBehaviour
{
    private IInputService _input;
    private WeaponInventory _inventory;
    private PlayerMovement _movement;
    private PlayerMount _mount;

    [Inject]
    public void Construct(IInputService input)
    {
        _input = input;
    }

    private void Awake()
    {
        _inventory = GetComponent<WeaponInventory>();
        _movement = GetComponent<PlayerMovement>();
        _mount = GetComponent<PlayerMount>();
    }

    private void Update()
    {
        if (!_input.AttackPressed)
            return;

        Vector2 direction = (_movement != null && !_movement.FacingRight) ? Vector2.left : Vector2.right;

        if (_mount != null && _mount.IsMounted)
        {
            _mount.Current.Attack(direction);
            return;
        }

        if (_inventory.HasWeapon)
            _inventory.Current.Attack(direction);
    }
}
