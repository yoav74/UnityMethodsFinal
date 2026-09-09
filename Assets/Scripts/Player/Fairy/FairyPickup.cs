using UnityEngine;
using Zenject;

/// <summary>
/// The fairy: a collectible (base <see cref="Pickup"/>) that, on contact, grants the player the
/// timed invincibility window. It only starts the window; the "unharmed" and "destroys everything
/// on contact" effects live on the player (<see cref="PlayerLifeline"/> and <see cref="FairyAura"/>).
/// </summary>
public class FairyPickup : Pickup
{
    private IInvincibility _invincibility;

    [Inject]
    public void Construct(IInvincibility invincibility)
    {
        _invincibility = invincibility;
    }

    protected override void OnCollected(GameObject player)
    {
        _invincibility?.Activate();
    }
}
