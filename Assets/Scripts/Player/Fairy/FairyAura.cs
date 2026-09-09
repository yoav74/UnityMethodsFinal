using UnityEngine;
using Zenject;

/// <summary>
/// While the fairy's invincibility is active, anything the player touches is destroyed —
/// enemies (via <see cref="Enemy.Kill"/>, ghosts included) and destructible hazards — rocks
/// via <see cref="IDestructible"/> and bonfires via <see cref="IFairyDestructible"/>. Sits on the player and does nothing when the
/// window is off, so ordinary contact behaves normally.
/// </summary>
public class FairyAura : MonoBehaviour
{
    private IInvincibility _invincibility;

    [Inject]
    public void Construct(IInvincibility invincibility)
    {
        _invincibility = invincibility;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_invincibility == null || !_invincibility.IsActive)
            return;

        other.GetComponent<Enemy>()?.Kill();              // any enemy, ghost included
        other.GetComponent<IDestructible>()?.Hit();          // rocks
        other.GetComponent<IFairyDestructible>()?.DestroyByFairy(); // bonfires (fairy only)
    }
}
