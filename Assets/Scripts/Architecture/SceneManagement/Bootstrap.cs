using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Start()
    {
        SceneLoader sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
        sceneLoader
            .NewTransition()
            .Load(SceneDatabase.Scenes.TitleScreen, SceneDatabase.GetSceneName(SceneDatabase.Scenes.TitleScreen), true)
            .Perform();
    }

}
