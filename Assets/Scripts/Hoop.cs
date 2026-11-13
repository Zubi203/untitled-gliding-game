using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hoop : MonoBehaviour
{
    public UnityEvent OnPlayerEntered;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float scoreGainMultiplier = 10f;
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

        ExpireHoop();
    }

    void ExpireHoop()
    {
        OnPlayerEntered?.Invoke();

        float scoreToGain = scoreData.baseScoreGain * scoreGainMultiplier;
        scoreData.ChangeScore((int)scoreToGain);
        
        Destroy(gameObject);
    }
}
