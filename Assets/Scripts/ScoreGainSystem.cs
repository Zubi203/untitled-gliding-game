using UnityEngine;

public class ScoreGainSystem : MonoBehaviour
{
    enum WallDirections
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back,
    }

    [SerializeField] private float wallDetectionRange = 20f;
    [SerializeField] private float minScoreMultiplier = 0.5f;
    [SerializeField] private float maxScoreMultiplier = 2.0f;
    [SerializeField] private string wallTag = "Wall";
    private BoxCollider wallDetectionTrigger;
    private ScoreData scoreData;

    void Awake()
    {
        scoreData = Resources.Load<ScoreData>("ScoreData");
        scoreData.ResetScore();
        CreateWallDetectionArea();
    }

    void CreateWallDetectionArea()
    {
        wallDetectionTrigger = gameObject.AddComponent<BoxCollider>();
        wallDetectionTrigger.isTrigger = true;
        wallDetectionTrigger.size = new Vector3(wallDetectionRange, wallDetectionRange, wallDetectionRange);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag(wallTag)) return;

        //increase score
        scoreData.ChangeScore(scoreData.baseScoreGain);
    }


}
