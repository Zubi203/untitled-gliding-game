using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Animations.Rigging;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerProceduralAnimation : MonoBehaviour
{
    [SerializeField] private PlayerInput inputHandler;
    [SerializeField] private float legMaxDampDistance = 3f;
    [SerializeField] private float maxHeadTargetExtendDistance = 4f;
    [SerializeField] private float minHeadTargetExtendDistance = 2.5f;
    [SerializeField] private float wingRotateSpeed = 50f;

    [Header("Constraints")]
    [SerializeField] private TwoBoneIKConstraint leftWingIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightWingIKConstraint;
    [SerializeField] private TwoBoneIKConstraint headIKConstraint;
    [SerializeField] private ChainIKConstraint tailIKConstraint;
    [SerializeField] private Rig wingSpreadRig;
    [SerializeField] private DampedTransform leftLegDampConstraint;
    [SerializeField] private DampedTransform rightLegDampConstraint;

    [Header("Target Transforms")]
    [SerializeField] private Transform leftWingIdleTarget;
    [SerializeField] private Transform rightWingIdleTarget;
    [SerializeField] private Transform leftWingFlapStartTarget;
    [SerializeField] private Transform rightWingFlapStartTarget;
    [SerializeField] private Transform leftWingFlapEndTarget;
    [SerializeField] private Transform rightWingFlapEndTarget;
    [SerializeField] private Transform currentLeftWingTarget;
    [SerializeField] private Transform currentRightWingTarget;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform neckBase;

    [Header("Tail")]
    [SerializeField] private Transform tailParent;
    [SerializeField] private float distanceBetweenTailSegments = 1f;
    [SerializeField] private Transform tailAttachPoint;
    [SerializeField] private float tailAngleThreshold = 30f;
    private List<Transform> tailSegments = new List<Transform>();
    private Vector3 leftLegOriginPos, rightLegOriginPos;
    private Quaternion leftWingRotation, rightWingRotation;
    

    //Read-only fields (to be accessed by the state machine)
    public Rig WingSpreadRig => wingSpreadRig;

    void Awake()
    {
        AssertReferences();
        GetTailSegments();

        if (leftLegDampConstraint == null || rightLegDampConstraint == null) return;

        leftLegOriginPos = leftLegDampConstraint.data.sourceObject.localPosition;
        rightLegOriginPos = rightLegDampConstraint.data.sourceObject.localPosition;
    }

    private void GetTailSegments()
    {
        if (tailParent == null) return;
        tailSegments.Clear();
        Transform[] tailChildren = tailParent.GetComponentsInChildren<Transform>().Where(t => t != tailParent).ToArray();

        foreach (Transform child in tailChildren)
        {
            tailSegments.Add(child);
        }
    }

    public void LookAtRotationDirection()
    {
        if (headTarget == null || neckBase == null || inputHandler == null) return;

        Vector3 neckDir = Quaternion.Euler(inputHandler.rotationInput) * Vector3.forward;
        headTarget.position = neckBase.position + (neckDir * maxHeadTargetExtendDistance);

        // constrain head position if its too close to neck
        Vector3 neckPos = neckBase.position;
        Vector3 headPos = headTarget.position;

        float distance = Vector3.Distance(neckPos, headPos);
        if (distance < minHeadTargetExtendDistance)
        {
            Vector3 extendDir = headPos - neckPos;
            Vector3 newPosition = (extendDir.normalized * minHeadTargetExtendDistance) + neckPos;
            headTarget.position = newPosition;
        }
    }

    public void WingsToIdle(float lerpSpeed = 10f)
    {
        currentLeftWingTarget.position = Vector3.Lerp(currentLeftWingTarget.position, leftWingIdleTarget.position, Time.deltaTime * lerpSpeed);
        currentRightWingTarget.position = Vector3.Lerp(currentRightWingTarget.position, rightWingIdleTarget.position, Time.deltaTime * lerpSpeed);

        leftWingRotation = leftWingIdleTarget.rotation;
        rightWingRotation =  rightWingIdleTarget.rotation;
    }

    public void WingsToFlapStart(float lerpPercent)
    {
        
        currentLeftWingTarget.position = Vector3.Lerp(leftWingIdleTarget.position, leftWingFlapStartTarget.position, lerpPercent);
        currentRightWingTarget.position = Vector3.Lerp(rightWingIdleTarget.position, rightWingFlapStartTarget.position, lerpPercent);

        leftWingRotation = leftWingFlapStartTarget.rotation;
        rightWingRotation = rightWingFlapStartTarget.rotation;
    }

    public void WingsToFlapEnd(float lerpPercent)
    {
        float idleLerpThreshold = 0.3f;
        if (lerpPercent < idleLerpThreshold)
        {
            float newLerpPercent = lerpPercent / idleLerpThreshold;
            currentLeftWingTarget.position = Vector3.Lerp(leftWingFlapStartTarget.position, leftWingIdleTarget.position, newLerpPercent);
            currentRightWingTarget.position = Vector3.Lerp(rightWingFlapStartTarget.position, rightWingIdleTarget.position, newLerpPercent);

            leftWingRotation = leftWingIdleTarget.rotation;
            rightWingRotation = rightWingIdleTarget.rotation;
        }
        else
        {
            float newLerpPercent = (lerpPercent - idleLerpThreshold) / (1f - idleLerpThreshold);
            currentLeftWingTarget.position = Vector3.Lerp(leftWingIdleTarget.position, leftWingFlapEndTarget.position, newLerpPercent);
            currentRightWingTarget.position = Vector3.Lerp(rightWingIdleTarget.position, rightWingFlapEndTarget.position, newLerpPercent);

            leftWingRotation = leftWingFlapEndTarget.rotation;
            rightWingRotation = rightWingFlapEndTarget.rotation;
        }
        
    }

    private void AssertReferences()
    {
        Assert.IsNotNull(leftWingIKConstraint, "The left wing IK constraint has not been assigned");
        Assert.IsNotNull(rightWingIKConstraint, "The right wing IK constraint has not been assigned");
        Assert.IsNotNull(tailIKConstraint, "The tail IK constraint has not been assigned");

        Assert.IsNotNull(leftWingIdleTarget, "The left wing idle target has not been assigned");
        Assert.IsNotNull(rightWingIdleTarget, "The right wing idle target has not been assigned");
        Assert.IsNotNull(leftWingFlapStartTarget, "The left wing flap start target has not been assigned");
        Assert.IsNotNull(rightWingFlapStartTarget, "The right wing flap start target has not been assigned");
        Assert.IsNotNull(leftWingFlapEndTarget, "The left wing flap end target has not been assigned");
        Assert.IsNotNull(rightWingFlapEndTarget, "The right wing flap end target has not been assigned");
        Assert.IsNotNull(wingSpreadRig, "The wing spread rig has not been assigned");
    }

    private void FixedUpdate()
    {
        ConstrainLeg(leftLegDampConstraint);
        ConstrainLeg(rightLegDampConstraint);
        MoveWingsToTarget();

        if (headTarget == null) return;
        headIKConstraint.data.target.position = headTarget.position;
        headIKConstraint.data.target.rotation = headTarget.rotation;
    }

    private void Update()
    {
        
        EvaluateTailConstraints();
        EvaluateTailRotation();
    }

    private void MoveWingsToTarget()
    {
        leftWingIKConstraint.data.target.position = currentLeftWingTarget.position;
        rightWingIKConstraint.data.target.position = currentRightWingTarget.position;


        currentLeftWingTarget.rotation = Quaternion.RotateTowards(currentLeftWingTarget.rotation, leftWingRotation, wingRotateSpeed * Time.deltaTime);
        currentRightWingTarget.rotation = Quaternion.RotateTowards(currentRightWingTarget.rotation, rightWingRotation, wingRotateSpeed * Time.deltaTime);
        leftWingIKConstraint.data.target.rotation = currentLeftWingTarget.rotation;
        rightWingIKConstraint.data.target.rotation = currentRightWingTarget.rotation;
    }

    private void EvaluateTailConstraints()
    {
        if (tailAttachPoint == null) return;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            Transform currentSegment = tailSegments[i];
            Transform prevSegment = (i == 0) ? tailAttachPoint : tailSegments[i - 1];

            float distance = Vector3.Distance(currentSegment.position, prevSegment.position);
            if (distance > distanceBetweenTailSegments || distance < distanceBetweenTailSegments)
            {
                Vector3 dirFromPrevious = currentSegment.position - prevSegment.position;
                currentSegment.position = prevSegment.position + (dirFromPrevious.normalized * distanceBetweenTailSegments);
            }
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

            Vector3 constrainedDirection = ClampAngleBetweenVectors(referenceDirection, rawDirection, tailAngleThreshold);

            tailSegments[i].position = prevSegment - (constrainedDirection * distanceBetweenTailSegments);
            
            tailSegments[i].rotation = Quaternion.LookRotation(constrainedDirection);
        }
    }

    Vector3 ClampAngleBetweenVectors(Vector3 fromDir, Vector3 toDir, float maxAngle)
    {
        Quaternion rotationDiff = Quaternion.FromToRotation(fromDir, toDir);
        rotationDiff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > maxAngle)
        {
            return Quaternion.AngleAxis(maxAngle, axis) * fromDir;
        }

        return toDir;
    }

    private void ConstrainLeg(DampedTransform legToConstrain)
    {
        if (legToConstrain == null) return;

        Vector3 originPoint = (legToConstrain == leftLegDampConstraint) ? leftLegOriginPos : rightLegOriginPos;
        Vector3 currentPos = legToConstrain.data.constrainedObject.localPosition;

        float distance = Vector3.Distance(originPoint, currentPos);

        if (distance > legMaxDampDistance)
        {
            Vector3 constrainedDirection = currentPos - originPoint;
            Vector3 newPosition = (constrainedDirection.normalized * legMaxDampDistance) + originPoint;
            legToConstrain.data.constrainedObject.localPosition = newPosition;
        }
        
    }
}
