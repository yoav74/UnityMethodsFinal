using UnityEngine;

/// <summary>
/// Game-wide tunable values from the design brief, authored as a ScriptableObject and
/// handed to systems through dependency injection — so nothing reaches for a singleton
/// or a Resources lookup to read them (Dependency Inversion).
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Lives")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int fruitsPerExtraLife = 20;

    [Header("Power")]
    [SerializeField] private int startingPower = 10;
    [Tooltip("Seconds between each single point of power lost to the timer.")]
    [SerializeField] private float powerDrainSeconds = 2f;
    [Tooltip("Power lost when the player walks into a rock.")]
    [SerializeField] private int rockPowerCost = 3;

    [Header("Fairy")]
    [Tooltip("Seconds of invincibility the fairy grants (brief: 10).")]
    [SerializeField] private float fairyInvincibilitySeconds = 10f;

    /// <summary>Lives the player starts a run with (brief: 3).</summary>
    public int StartingLives => startingLives;

    /// <summary>Fruits that must be collected to earn one extra life (brief: 20).</summary>
    public int FruitsPerExtraLife => fruitsPerExtraLife;

    /// <summary>Power the player starts each level with.</summary>
    public int StartingPower => startingPower;

    /// <summary>Seconds between each point of power drained over time.</summary>
    public float PowerDrainSeconds => powerDrainSeconds;

    /// <summary>Power lost on hitting a rock (brief: 3).</summary>
    public int RockPowerCost => rockPowerCost;

    /// <summary>Seconds of fairy invincibility (brief: 10).</summary>
    public float FairyInvincibilitySeconds => fairyInvincibilitySeconds;
}
