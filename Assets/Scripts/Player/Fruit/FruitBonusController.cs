using Zenject;

/// <summary>
/// Grants an extra life for every <see cref="GameConfig.FruitsPerExtraLife"/> fruits collected
/// (the brief: 20 → +1 life). It listens to the <see cref="FruitCounter"/> and talks to the
/// <see cref="LivesModel"/>, keeping the counter ignorant of lives and the lives model ignorant
/// of fruit (Single Responsibility). Built by the container as an <see cref="IInitializable"/>,
/// project-scoped like the state it bridges.
/// </summary>
public class FruitBonusController : IInitializable, System.IDisposable
{
    private readonly FruitCounter _fruits;
    private readonly LivesModel _lives;
    private readonly int _fruitsPerLife;

    public FruitBonusController(FruitCounter fruits, LivesModel lives, GameConfig config)
    {
        _fruits = fruits;
        _lives = lives;
        _fruitsPerLife = config.FruitsPerExtraLife;
    }

    public void Initialize()
    {
        _fruits.CountChanged += OnCountChanged;
    }

    public void Dispose()
    {
        _fruits.CountChanged -= OnCountChanged;
    }

    private void OnCountChanged(int total)
    {
        // Every whole multiple of the threshold (20, 40, …) awards one life; the HUD can show
        // progress toward the next as total % threshold.
        if (_fruitsPerLife > 0 && total > 0 && total % _fruitsPerLife == 0)
            _lives.GainLife();
    }
}
