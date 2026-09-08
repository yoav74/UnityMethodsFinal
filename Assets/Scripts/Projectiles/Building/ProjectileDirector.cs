/// <summary>
/// <b>Director</b> for the Builder pattern: drives a <see cref="ProjectileBuilder"/> through
/// the fixed construction sequence (speed → lifetime → size → build) using the values it is
/// given, so callers get a consistently-assembled projectile without repeating the steps.
/// The values come from the factory, keeping per-weapon tuning in one place.
/// </summary>
public class ProjectileDirector
{
    private readonly ProjectileBuilder _builder;

    public ProjectileDirector(ProjectileBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>Assembles a projectile, step by step, from the given values.</summary>
    public BaseProjectile Build(float speed, float lifetime, float size)
    {
        return _builder
            .SetSpeed(speed)
            .SetLifetime(lifetime)
            .SetSize(size)
            .Build();
    }
}
