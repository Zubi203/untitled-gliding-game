using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SceneDatabase
{
    public enum Scenes
    {
        TitleScreen,
        TestLevel
    }

    static Dictionary<Scenes, string> scenePaths = new Dictionary<Scenes, string>
    {
        {Scenes.TitleScreen , "TitleScreen"},
        {Scenes.TestLevel , "TestScene"}
    };

    public static string GetSceneName(Scenes targetScene)
    {
        return scenePaths[targetScene];
    }

    public static int GetSceneBuildIndex(Scenes targetScene)
    {
        return SceneUtility.GetBuildIndexByScenePath(scenePaths[targetScene]);
    }
}
