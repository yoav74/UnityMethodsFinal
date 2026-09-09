using UnityEngine;
using Zenject;

/// <summary>
/// <b>Factory</b> for the collectible an egg reveals. It hides the choice + creation behind
/// <see cref="Create"/>: a forced pick (a specific collectible, for testing/showcasing) takes
/// priority; otherwise it draws randomly from a pool; if neither is set it reveals nothing. It
/// builds the prefab through Zenject's <see cref="IInstantiator"/> so injected collectibles (the
/// fairy, fruit) get their dependencies even though they are spawned at runtime.
/// </summary>
public class CollectibleFactory
{
    private readonly IInstantiator _instantiator;
    private readonly GameObject _forced;
    private readonly GameObject[] _randomPool;

    public CollectibleFactory(IInstantiator instantiator, GameObject forced, GameObject[] randomPool)
    {
        _instantiator = instantiator;
        _forced = forced;
        _randomPool = randomPool;
    }

    /// <summary>Reveal the collectible at <paramref name="position"/>, or null for "nothing".</summary>
    public GameObject Create(Vector3 position)
    {
        GameObject prefab = _forced;
        if (prefab == null && _randomPool != null && _randomPool.Length > 0)
            prefab = _randomPool[Random.Range(0, _randomPool.Length)];

        if (prefab == null)
            return null;

        return _instantiator.InstantiatePrefab(prefab, position, Quaternion.identity, null);
    }
}
