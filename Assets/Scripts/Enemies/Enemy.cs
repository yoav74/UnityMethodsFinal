using UnityEngine;
using Zenject;

/// <summary>
/// Base for every enemy: owns health, the shared damage/death path, and the "touching the player
/// hurts them" rule, leaving movement/AI to subclasses (Open/Closed — each specific enemy extends
/// this). Killable by weapons and mount attacks through <see cref="IDamageable"/>; the ghost
/// (ME-63) overrides <see cref="TakeDamage"/> to shrug those off. On death it raises
/// <see cref="Died"/> — the hook the respawn timer (ME-56) and drops (ME-57) attach to — then
/// removes itself via <see cref="OnDeath"/>.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] private string playerTag = "Player";

    protected int Health { get; private set; }

    private IDeathService _death;

    /// <summary>Raised when this enemy dies (respawn timer / drop-on-death listen here).</summary>
    public event System.Action<Enemy> Died;

    [Inject]
    public void Construct(IDeathService death)
    {
        _death = death;
    }

    protected virtual void Awake()
    {
        Health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        if (Health <= 0)
            return;

        Health -= amount;
        if (Health <= 0)
            Die();
    }

    protected void Die()
    {
        Died?.Invoke(this);
        OnDeath();
    }

    /// <summary>How the enemy leaves play once dead. Default removes it; subclasses may override.</summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            _death?.Die();
    }
}
