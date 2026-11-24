using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    [SerializeField] private float scorePoolWaitTime = 1f;
    private float scorePoolWaitCountDown, scoreGainTimer;

    [Header("Score Based On Surrounding Walls")]
    [SerializeField] private float maxSurroundingWallMultiplier = 7f;
    [SerializeField] private AnimationCurve scoreGainBySurroundingWalls;
    [SerializeField] private float wallDetectionRange = 20f;
    [SerializeField] private string wallTag = "Wall";

    [Header("Score Over Time")]
    [SerializeField] private float maxOverTimeMultiplier = 2f;
    [SerializeField] private AnimationCurve scoreGainOverTime;
    [SerializeField] private float timeToMaxScore = 3f;

    [Header("Score By Proximity to Wall")]
    [SerializeField] private float maxWallProximityMultiplier = 2f;
    [SerializeField] private AnimationCurve scoreGainOverDistanceFromWall;

    [Header("Score Over Speed")]
    [SerializeField] private float maxSpeedScoreMultiplier = 2f;
    [SerializeField] private AnimationCurve scoreGainBySpeed;
    [SerializeField] private float playerMaxSpeed = 200f;
    private SphereCollider wallDetectionTrigger;
    private ScoreData scoreData;
    private List<Collider> overlappingWalls = new List<Collider>();
    private Dictionary<WallDirections, bool> hasWallInDirection = new Dictionary<WallDirections, bool>{
        {WallDirections.Back, false},
        {WallDirections.Forward, false},
        {WallDirections.Right, false},
        {WallDirections.Left, false},
        {WallDirections.Up, false},
        {WallDirections.Down, false}
    };
    private bool isCloseToWall = false;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        scoreData = Resources.Load<ScoreData>(nameof(ScoreData));
        scoreData.ResetScorePool();
        scoreData.ResetTotalScore();
        CreateWallDetectionArea();

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            playerRigidbody = rb;
        }
    }

    private void Update()
    {
        CheckWallDirections();

        isCloseToWall = overlappingWalls.Count > 0;

        if (!isCloseToWall)
        {
            scoreGainTimer = 0f;

            if (scorePoolWaitCountDown <= 0f)
            {
                scoreData.AddPoolToTotal();
                scorePoolWaitCountDown = scorePoolWaitTime;
            }
            else
            {
                scorePoolWaitCountDown -= Time.deltaTime;
            }
        }
        else
        {
            IncrementScorePool();

            scorePoolWaitCountDown = scorePoolWaitTime;

            if (scoreGainTimer >= timeToMaxScore)
            {
                scoreGainTimer = timeToMaxScore;
            }
            else
            {
                scoreGainTimer += Time.deltaTime;
            }
        }
    }

    private void CreateWallDetectionArea()
    {
        wallDetectionTrigger = gameObject.AddComponent<SphereCollider>();
        wallDetectionTrigger.isTrigger = true;
        wallDetectionTrigger.radius = wallDetectionRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(wallTag)) return;

        if (!overlappingWalls.Contains(other))
        {
            overlappingWalls.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(wallTag)) return;

        if (overlappingWalls.Contains(other))
        {
            overlappingWalls.Remove(other);
        }
    }

    private void IncrementScorePool()
    {
        float wallDirectionMultiplier = GetWallDirectionScoreMultiplier();
        float wallProximityMultiplier = GetWallProximityScoreMultiplier();
        float overTimeMultiplier = GetScoreGainOverTimeMultiplier();
        float speedScoreMultiplier = GetSpeedScoreMultiplier();

        Debug.Log(wallDirectionMultiplier);

        float scoreGain = (float)scoreData.BaseScoreGain * overTimeMultiplier * wallDirectionMultiplier * wallProximityMultiplier * speedScoreMultiplier;
        scoreData.ChangeScorePool((int)scoreGain);
    }

    public void DeleteScorePool()
    {
        scoreData.ResetScorePool();
    }

    private void CheckWallDirections()
    {
        foreach (WallDirections direction in hasWallInDirection.Keys.ToList())
        {
            Vector3 RaycastDir = Vector3.zero;
            switch (direction)
            {
                case WallDirections.Back:
                    RaycastDir = -transform.forward;
                    break;
                case WallDirections.Left:
                    RaycastDir = -transform.right;
                    break;
                case WallDirections.Down:
                    RaycastDir = -transform.up;
                    break;
                case WallDirections.Forward:
                    RaycastDir = transform.forward;
                    break;
                case WallDirections.Right:
                    RaycastDir = transform.right;
                    break;
                case WallDirections.Up:
                    RaycastDir = transform.up;
                    break;
            }

            if (Physics.Raycast(transform.position, RaycastDir, out RaycastHit hit, wallDetectionRange))
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    hasWallInDirection[direction] = true;
                }
            }
            else
            {
                hasWallInDirection[direction] = false;
            }
        }
    }

    private float GetWallDirectionScoreMultiplier()
    {
        float scorePerWall = (maxSurroundingWallMultiplier - 1f) / (float)hasWallInDirection.Keys.Count;
        float scoreMultiplier = 1f;

        foreach (WallDirections direction in hasWallInDirection.Keys.ToList())
        {
            if (hasWallInDirection[direction])
            {
                scoreMultiplier += scorePerWall;
            }
        }
        return Mathf.Clamp(scoreMultiplier, 1f, maxSurroundingWallMultiplier);
    }

    private float GetScoreGainOverTimeMultiplier()
    {
        float timeElapsedPercent = scoreGainTimer / timeToMaxScore;
        float scoreMultiplier = scoreGainOverTime.Evaluate(timeElapsedPercent) * (maxOverTimeMultiplier - 1f);
        scoreMultiplier += 1f;
        return Mathf.Clamp(scoreMultiplier, 1f, maxOverTimeMultiplier);
    }

    private float GetWallProximityScoreMultiplier()
    {
        if (overlappingWalls.Count == 0) return 1f;

        List<Vector3> closestPoints = new List<Vector3>();
        foreach (Collider wall in overlappingWalls)
        {
            Vector3 point = wall.ClosestPoint(transform.position);
            closestPoints.Add(point);
        }

        Vector3 closestPoint = Vector3.zero;
        float closestDist = 99999f;
        foreach (Vector3 point in closestPoints)
        {
            if (Vector3.Distance(transform.position, point) < closestDist)
            {
                closestDist = Vector3.Distance(transform.position, point);
                closestPoint = point;
            }
        }

        float distancePercent = Vector3.Distance(transform.position, closestPoint) / wallDetectionRange;
        float proximityScoreMultiplier = scoreGainOverDistanceFromWall.Evaluate(1f - distancePercent) * maxWallProximityMultiplier;
        return Mathf.Clamp(proximityScoreMultiplier, 0f, maxWallProximityMultiplier);
    }

    private float GetSpeedScoreMultiplier()
    {
        if (playerRigidbody == null) return 1f;

        float speedPercent = playerRigidbody.linearVelocity.magnitude / playerMaxSpeed;
        float scoreMultiplier =  scoreGainBySpeed.Evaluate(speedPercent) * maxSpeedScoreMultiplier;
        return Mathf.Clamp(scoreMultiplier, 0f, maxSpeedScoreMultiplier);
    }


}
