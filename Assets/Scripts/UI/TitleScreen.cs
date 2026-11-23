using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        SceneLoader sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
        sceneLoader
            .NewTransition()
            .Load(SceneDatabase.Scenes.TestLevel, SceneDatabase.GetSceneName(SceneDatabase.Scenes.TestLevel), true)
            .UseLoadingOverlay()
            .Unload(SceneDatabase.Scenes.TitleScreen)
            .Perform();
    }
}
