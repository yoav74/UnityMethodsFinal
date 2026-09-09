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

    [Tooltip("The player's own body sprite, hidden while mounted so only the animal shows. Auto-found if left empty.")]
    [SerializeField] private SpriteRenderer riderSprite;

    private GameObject _currentVisual;

    private void Awake()
    {
        if (riderSprite == null)
            riderSprite = GetComponent<SpriteRenderer>();
    }

    public void Mount(IMount mount)
    {
        var visual = (mount as MonoBehaviour) != null ? ((MonoBehaviour)mount).gameObject : null;

        if (_currentVisual != null && _currentVisual != visual)
            Destroy(_currentVisual); // swap: the old animal disappears

        Current = mount;
        _currentVisual = visual;
        SetRiderVisible(false); // while riding you ARE the animal — hide the player body sprite
        MountChanged?.Invoke(Current);
    }

    public void Dismount()
    {
        if (_currentVisual != null)
            Destroy(_currentVisual);

        _currentVisual = null;
        Current = null;
        SetRiderVisible(true); // back on foot — show the player again
        MountChanged?.Invoke(null);
    }

    private void SetRiderVisible(bool visible)
    {
        if (riderSprite != null)
            riderSprite.enabled = visible;
    }
}
