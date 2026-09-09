using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// <b>Factory</b> for the collectible an egg reveals. It hides the choice + creation behind
/// <see cref="Create"/>: a forced pick (a specific collectible, for testing/showcasing) takes
/// priority; otherwise it draws randomly from a pool; if neither is set it reveals nothing. It works
/// in terms of <see cref="Pickup"/> — the shared "pickupable item" type — so only real collectibles
/// (weapons, animals, the fairy) can ever be produced, not arbitrary prefabs. It builds through
/// Zenject's <see cref="IInstantiator"/> so injected collectibles (the fairy) get their dependencies
/// even though they are spawned at runtime, then lets the reveal fall to the ground.
/// </summary>
public class CollectibleFactory
{
    private readonly IInstantiator _instantiator;
    private readonly Pickup _forced;
    private readonly IReadOnlyList<Pickup> _randomPool;

    public CollectibleFactory(IInstantiator instantiator, Pickup forced, IReadOnlyList<Pickup> randomPool)
    {
        _instantiator = instantiator;
        _forced = forced;
        _randomPool = randomPool;
    }

    /// <summary>Reveal the collectible at <paramref name="position"/>, or null for "nothing".</summary>
    public Pickup Create(Vector3 position)
    {
        Pickup prefab = _forced;
        if (prefab == null && _randomPool != null && _randomPool.Count > 0)
            prefab = _randomPool[Random.Range(0, _randomPool.Count)];

        if (prefab == null)
            return null;

        Pickup reveal = _instantiator.InstantiatePrefabForComponent<Pickup>(
            prefab.gameObject, position, Quaternion.identity, null);
        FallToGround.Attach(reveal.gameObject); // the reveal drops to the ground
        return reveal;
    }
}
