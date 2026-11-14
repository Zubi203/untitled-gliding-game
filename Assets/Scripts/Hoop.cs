using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hoop : MonoBehaviour
{
    public UnityEvent OnPlayerEntered;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float scoreGainMultiplier = 10f;
    [SerializeField] private ParticleSystem collectParticles;
    private Collider detectionArea;
    private ScoreData scoreData;

    void Awake()
    {
        detectionArea = GetComponent<Collider>();
        detectionArea.isTrigger = true;

        scoreData = Resources.Load<ScoreData>(nameof(ScoreData));
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(playerTag)) return;
        if (other.isTrigger) return;

        ExpireHoop();
    }

    void ExpireHoop()
    {
        detectionArea.enabled = false;
        
        OnPlayerEntered?.Invoke();

        if (collectParticles != null)
        {
            collectParticles.Play();
        }

        float scoreToGain = scoreData.baseScoreGain * scoreGainMultiplier;
        scoreData.ChangeScore((int)scoreToGain);
        
        if (TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
        {
            mesh.enabled = false;
        }
    }
}
