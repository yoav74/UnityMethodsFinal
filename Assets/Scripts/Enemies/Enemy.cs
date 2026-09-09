using UnityEngine;

/// <summary>
/// Base for every enemy — <b>damageable or not</b>. It owns only what all enemies share: the
/// "touching the player hurts them" rule, the one-shot death guard, the <see cref="Died"/> hook
/// (respawn timer ME-56, drops ME-57 listen here) and how a dead enemy leaves play. It deliberately
/// does <b>not</b> know about health or <see cref="IDamageable"/>: enemies that can be damaged add
/// that through <see cref="DamageableEnemy"/>, while the ghost (ME-63) extends this directly and is
/// simply not <see cref="IDamageable"/> at all — so weapons and mounts find nothing to damage on it,
/// with no no-op override needed (Interface Segregation / Liskov).
///
/// <see cref="Kill"/> is the single death entry point: the fairy calls it to force a kill (a ghost
/// included), and <see cref="DamageableEnemy"/> calls it when health runs out. Touching the player
/// routes through the player's <see cref="IKillable"/>, so the fairy's invincibility can spare them
/// without the enemy knowing anything about it.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected string playerTag = "Player";

    private bool _dead;

    /// <summary>Raised when this enemy dies (respawn timer / drop-on-death listen here).</summary>
    public event System.Action<Enemy> Died;

    // Reset the death guard on every (re)activation, so a respawned enemy is alive again.
    protected virtual void OnEnable()
    {
        _dead = false;
    }

    /// <summary>
    /// The one and only way an enemy dies: the fairy's contact kill and health-depletion both come
    /// through here. Fires once, raises <see cref="Died"/>, then removes the enemy via
    /// <see cref="OnDeath"/>.
    /// </summary>
    public void Kill()
    {
        if (_dead)
            return;

        _dead = true;
        Died?.Invoke(this);
        OnDeath();
    }

    /// <summary>
    /// How the enemy leaves play once dead: it deactivates rather than destroys, so a respawn
    /// handler (<see cref="RespawnOnDeath"/>, ME-56) can bring the very same instance back at its
    /// spot. Enemies with no respawn simply stay gone. Subclasses may override.
    /// </summary>
    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            other.GetComponent<IKillable>()?.Kill();
    }
}
