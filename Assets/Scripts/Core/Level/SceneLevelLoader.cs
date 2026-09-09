using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <see cref="ILevelLoader"/> backed by Unity's SceneManager, using <b>async scene loading</b>
/// (Async &amp; Tasks): each transition awaits the <see cref="AsyncOperation"/> to completion. The
/// ordered level list is the build-settings scene order — <see cref="LoadNext"/> advances by build
/// index and wraps to the first past the end. A plain C# service built by the container (DI).
///
/// Loading a level rebuilds its scene container, so per-level state (power) is created fresh, while
/// project-scoped state (lives, fruit) survives across the transition — that is what makes
/// "retry the level with the lives you have left" and the cross-level fruit bonus work.
/// </summary>
public class SceneLevelLoader : ILevelLoader
{
    private const int FirstLevelBuildIndex = 0;

    public void ReloadCurrent()
    {
        LoadAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadFirst()
    {
        LoadAsync(FirstLevelBuildIndex);
    }

    public void LoadNext()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        LoadAsync(next < SceneManager.sceneCountInBuildSettings ? next : FirstLevelBuildIndex);
    }

    private async void LoadAsync(int buildIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        while (operation != null && !operation.isDone)
            await Task.Yield();
    }
}
