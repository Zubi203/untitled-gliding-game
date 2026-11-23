using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DestroyOnCrash : MonoBehaviour
{
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float sceneReloadDelay = 2f;
    [SerializeField] private GameObject[] playerVisuals;
    [SerializeField] private ParticleSystem deathParticles;
    private bool isReloadingScene;

    void Awake()
    {
        isReloadingScene = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTag))
        {
            Crash();
        }
    }

    void Crash()
    {
        if (deathParticles != null)
        {
            deathParticles.Play();
        }

        //screen shake
        if (TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            collider.enabled = false;
        }

        foreach (GameObject visual in playerVisuals)
        {
            visual.SetActive(false);
        }
        
        if(TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        
        StartCoroutine(ReloadCurrentScene());
    }

    IEnumerator ReloadCurrentScene()
    {
        if (isReloadingScene) yield break;

        isReloadingScene = true;
        yield return new WaitForSeconds(sceneReloadDelay);
        SceneLoader sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
        sceneLoader
            .NewTransition()
            .Load(SceneDatabase.Scenes.TestLevel, SceneDatabase.GetSceneName(SceneDatabase.Scenes.TestLevel), true)
            .UseLoadingOverlay()
            .Unload(SceneDatabase.Scenes.TestLevel)
            .Perform();
        isReloadingScene = false;
    }
}
