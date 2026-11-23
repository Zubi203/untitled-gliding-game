using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SceneDatabase: ScriptableObject
{
    public enum Scenes
    {
        TitleScreen,
        TestLevel
    }

    [SerializeField] private Dictionary<Scenes, string> scenePaths = new Dictionary<Scenes, string>
    {
        {Scenes.TitleScreen , ""},
        {Scenes.TestLevel , ""}
    };

    public string GetSceneName(Scenes targetScene)
    {
        return scenePaths[targetScene];
    }

    public int GetSceneBuildIndex(Scenes targetScene)
    {
        return SceneUtility.GetBuildIndexByScenePath(scenePaths[targetScene]);
    }
}
