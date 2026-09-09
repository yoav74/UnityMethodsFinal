using UnityEngine;

/// <summary>
/// Drops a collectible where an enemy dies (per the brief, usually an animal pickup). The drop is
/// <b>selectable per instance</b> — assign a specific prefab to force it, or leave it empty for no
/// drop — which keeps testing and showcasing deterministic; an optional chance gates random drops.
/// Listens to <see cref="Enemy.Died"/> and spawns at the enemy's position.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class DropOnDeath : MonoBehaviour
{
    [Tooltip("Collectible to drop on death (an animal pickup). Leave empty for no drop.")]
    [SerializeField] private GameObject dropPrefab;

    [Tooltip("Probability the drop appears (1 = always).")]
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 1f;

    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _enemy.Died += OnDied;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
            _enemy.Died -= OnDied;
    }

    private void OnDied(Enemy enemy)
    {
        if (dropPrefab == null || Random.value > dropChance)
            return;

        Instantiate(dropPrefab, transform.position, Quaternion.identity);
    }
}
