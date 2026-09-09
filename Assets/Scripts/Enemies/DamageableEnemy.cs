using UnityEngine;

/// <summary>
/// Base for every enemy that <b>can be hurt by weapons and mount attacks</b>: it adds health and the
/// <see cref="IDamageable"/> contract on top of <see cref="Enemy"/>, and routes a fatal hit into the
/// shared <see cref="Enemy.Kill"/> path. The spider, bird, snakes and frog extend this; the ghost
/// does not (it is not damageable at all). Splitting it this way keeps <see cref="IDamageable"/> off
/// the enemies that can't honour it (Interface Segregation), instead of one base that some subclass
/// has to neuter with an empty override.
/// </summary>
public abstract class DamageableEnemy : Enemy, IDamageable
{
    [SerializeField] protected int maxHealth = 1;

    protected int Health { get; private set; }

    protected virtual void Awake()
    {
        Health = maxHealth;
    }

    // Also reset on every (re)activation, so a respawned enemy comes back at full health.
    protected override void OnEnable()
    {
        base.OnEnable();
        Health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        if (Health <= 0)
            return;

        Health -= amount;
        if (Health <= 0)
            Kill(); // hand off to the shared death path (Died event + OnDeath)
    }
}
