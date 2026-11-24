using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class SceneLoader : Service
{
    #region Initialize
    void Awake()
    {
        RegisterToServiceLocator(this);
        Assert.IsNotNull(loadingScreen, "Loading Screen has not been assigned");
    }

    void OnDestroy()
    {
        UnregisterFromServiceLocator<SceneLoader>();
    }
    #endregion

    [SerializeField] private LoadingScreen loadingScreen;
    private Dictionary<SceneDatabase.Scenes, string> currentLoadedScenes = new Dictionary<SceneDatabase.Scenes, string>();
    private bool isBusy = false;

    public SceneTransitionPlan NewTransition()
    {
        return new SceneTransitionPlan();
    }

    public Coroutine ExecutePlan(SceneTransitionPlan plan)
    {
        if (isBusy)
        {
            Debug.LogWarning("Scene change already in progress");
            return null;
        }
        isBusy = true;

        return StartCoroutine(ChangeSceneRoutine(plan));
    }

    private IEnumerator ChangeSceneRoutine(SceneTransitionPlan plan)
    {
        if (plan.LoadingOverlay)
        {
            loadingScreen.StartTransition();
            yield return new WaitForSeconds(loadingScreen.TransitionDuration);
        }

        foreach (var slotKey in plan.scenesToUnload)
        {
            yield return UnloadSceneRoutine(slotKey);
        }

        if (plan.CleanUnusedAssets)
        {
            yield return CleanUnusedAssetsRoutine();
        }

        foreach (var kvp in plan.scenesToLoad)
        {
            if (currentLoadedScenes.ContainsKey(kvp.Key))
            {
                yield return UnloadSceneRoutine(kvp.Key);
            }
            yield return LoadAdditiveRoutine(kvp.Key, kvp.Value, plan.ActiveSceneName == kvp.Value);
        }

        if (plan.LoadingOverlay)
        {
            loadingScreen.EndTransition();
        }
        isBusy = false;
    }

    private IEnumerator LoadAdditiveRoutine(SceneDatabase.Scenes slotKey, string sceneName, bool setActive)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (loadOperation == null) yield break;

        loadOperation.allowSceneActivation = false;
        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }
        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        if (setActive)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid() && newScene.isLoaded)
            {
                SceneManager.SetActiveScene(newScene);
            }
        }
        currentLoadedScenes[slotKey] = sceneName;
    }

    private IEnumerator UnloadSceneRoutine(SceneDatabase.Scenes slotKey)
    {
        if (!currentLoadedScenes.TryGetValue(slotKey, out string sceneName)) yield break;
        if (string.IsNullOrEmpty(sceneName)) yield break;

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
        if (unloadOperation != null)
        {
            while(!unloadOperation.isDone)
            {
                yield return null;
            }
        }
        currentLoadedScenes.Remove(slotKey);
    }

    private IEnumerator CleanUnusedAssetsRoutine()
    {
        AsyncOperation cleanupOperation = Resources.UnloadUnusedAssets();
        while (!cleanupOperation.isDone)
        {
            yield return null;
        }
    }

    public class SceneTransitionPlan
    {
        public Dictionary<SceneDatabase.Scenes, string> scenesToLoad {get; } = new Dictionary<SceneDatabase.Scenes, string>();
        public List<SceneDatabase.Scenes> scenesToUnload {get; } = new List<SceneDatabase.Scenes>();
        public string ActiveSceneName {get; private set;} = "";
        public bool CleanUnusedAssets {get; private set;} = false;
        public bool LoadingOverlay {get; private set;} = false;

        public SceneTransitionPlan Load(SceneDatabase.Scenes slotKey, string sceneName, bool setActive = false)
        {
            scenesToLoad[slotKey] = sceneName;
            if (setActive) ActiveSceneName = sceneName;
            return this;
        }

        public SceneTransitionPlan Unload(SceneDatabase.Scenes slotKey)
        {
            scenesToUnload.Add(slotKey);
            return this;
        }

        public SceneTransitionPlan UseLoadingOverlay()
        {
            LoadingOverlay = true;
            return this;
        }

        public SceneTransitionPlan UseCleanUnusedAssets()
        {
            CleanUnusedAssets = true;
            return this;
        }

        public Coroutine Perform()
        {
            SceneLoader sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
            return sceneLoader.ExecutePlan(this);
        }

    }
}
