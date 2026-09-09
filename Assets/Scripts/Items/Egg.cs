using UnityEngine;
using Zenject;

/// <summary>
/// An egg the player opens on contact to reveal a collectible — an animal, a weapon, or the
/// fairy — produced by a <see cref="CollectibleFactory"/>. The reveal is selectable per instance:
/// set <see cref="forcedDrop"/> to force a specific collectible (or leave it empty for none),
/// which makes testing and showcasing deterministic; otherwise it draws from
/// <see cref="randomPool"/>. Extends the base <see cref="Pickup"/> so it detects the player and
/// removes itself; only its effect (spawning the reveal) differs.
/// </summary>
public class Egg : Pickup
{
    [Tooltip("Force this exact collectible to be revealed (leave empty for none / use the random pool).")]
    [SerializeField] private GameObject forcedDrop;

    [Tooltip("Revealed at random when no forced drop is set.")]
    [SerializeField] private GameObject[] randomPool;

    private IInstantiator _instantiator;

    [Inject]
    public void Construct(IInstantiator instantiator)
    {
        _instantiator = instantiator;
    }

    protected override void OnCollected(GameObject player)
    {
        var factory = new CollectibleFactory(_instantiator, forcedDrop, randomPool);
        factory.Create(transform.position);
    }
}
