using System;
using UnityEngine;

/// <summary>
/// The player's mount slot: which animal is being ridden, if any. Collecting an animal
/// <see cref="Mount"/>s it — swapping out (and removing) a previous mount — and hazards can
/// <see cref="Dismount"/> it. Raises <see cref="MountChanged"/> so the HUD and the attack can
/// react without polling.
/// </summary>
public class PlayerMount : MonoBehaviour
{
    public IMount Current { get; private set; }
    public bool IsMounted => Current != null;

    /// <summary>Raised when the mount changes (mounted, swapped, or dismounted to null).</summary>
    public event Action<IMount> MountChanged;

    private GameObject _currentVisual;

    public void Mount(IMount mount)
    {
        var visual = (mount as MonoBehaviour) != null ? ((MonoBehaviour)mount).gameObject : null;

        if (_currentVisual != null && _currentVisual != visual)
            Destroy(_currentVisual); // swap: the old animal disappears

        Current = mount;
        _currentVisual = visual;
        MountChanged?.Invoke(Current);
    }

    public void Dismount()
    {
        if (_currentVisual != null)
            Destroy(_currentVisual);

        _currentVisual = null;
        Current = null;
        MountChanged?.Invoke(null);
    }
}
