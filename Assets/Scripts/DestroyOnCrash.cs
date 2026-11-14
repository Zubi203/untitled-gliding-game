using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class DestroyOnCrash : MonoBehaviour
{
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float sceneReloadDelay = 2f;
    [SerializeField] private GameObject playerVisual;
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

        if (playerVisual != null)
        {
            playerVisual.SetActive(false);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isReloadingScene = false;
    }
}
