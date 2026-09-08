using UnityEngine.SceneManagement;

/// <summary>
/// <see cref="ILevelLoader"/> backed by Unity's SceneManager. A plain C# service built by the
/// container. Reloading a level rebuilds the scene container (so per-level state like power is
/// created fresh), while project-scoped state (lives) survives — that is what makes
/// "lose a life, retry the level with the lives you have left" work. The proper ordered level
/// list is introduced with the level flow (ME-66).
/// </summary>
public class SceneLevelLoader : ILevelLoader
{
    private const int FirstLevelBuildIndex = 0;

    public void ReloadCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadFirst()
    {
        SceneManager.LoadScene(FirstLevelBuildIndex);
    }
}
