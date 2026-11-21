using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class ProceduralTail : MonoBehaviour
{
    [SerializeField] private bool adjustSegmentScale = true;
    [SerializeField] private AnimationCurve tailScaleCurve;
    [SerializeField] private float tailScaleMultiplier = 0.5f;
    [SerializeField] private Transform tailParent;
    [SerializeField] private float distanceBetweenTailSegments = 1f;
    [SerializeField] private Transform tailAttachPoint;
    [SerializeField] private float tailAngleThreshold = 30f;
    private List<Transform> tailSegments = new List<Transform>();

    void Awake()
    {
        GetTailSegments();
    }

    private void Update()
    {
        EvaluateTailConstraints();
        EvaluateTailRotation();
    }

    private void GetTailSegments()
    {
        if (tailParent == null) return;
        tailSegments.Clear();

        for (int i = 0; i < tailParent.childCount; i++)
        {
            Transform child = tailParent.GetChild(i);

            tailSegments.Add(child);

            
        }
        if (!adjustSegmentScale) return;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            float tailSegmentPercent = (float)i / (float)tailSegments.Count;
            float tailSegmentScale = tailScaleCurve.Evaluate(tailSegmentPercent) * tailScaleMultiplier;
            Vector3 currentScale = tailSegments[i].transform.localScale;
            tailSegments[i].transform.localScale = new Vector3(currentScale.x * tailSegmentScale, currentScale.y * tailSegmentScale, currentScale.z * tailScaleMultiplier);
        }
    }

    private void EvaluateTailConstraints()
    {
        if (tailAttachPoint == null) return;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            Vector3 currentSegment = tailSegments[i].position;
            Vector3 prevSegment = (i == 0) ? tailAttachPoint.position : tailSegments[i - 1].position;
            Vector3 targetPosition = VectorSolver.ConstrainPosition(currentSegment, prevSegment, distanceBetweenTailSegments);
            tailSegments[i].position = targetPosition;

            
        }
    }

    

    

    private void EvaluateTailRotation()
    {
        for (int i = 0; i < tailSegments.Count; i++)
        {
            Vector3 currentSegment = tailSegments[i].position;
            Vector3 prevSegment = (i == 0) ? tailAttachPoint.position : tailSegments[i-1].position;
            
            Vector3 rawDirection = (prevSegment - currentSegment).normalized;
            
            Vector3 referenceDirection = (i <= 1) ? tailAttachPoint.forward : (tailSegments[i-2].position - tailSegments[i-1].position).normalized;

            Vector3 constrainedDirection = VectorSolver.ClampAngleBetweenVectors(referenceDirection, rawDirection, tailAngleThreshold);

            Vector3 targetPosition = prevSegment - (constrainedDirection * distanceBetweenTailSegments);
            tailSegments[i].position = targetPosition;
            
            tailSegments[i].rotation = Quaternion.LookRotation(constrainedDirection);
        }
    }
}
