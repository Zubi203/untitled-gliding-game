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
    [SerializeField] private float wingLerpSpeed = 10f;

    [Header("Constraints")]
    [SerializeField] private TwoBoneIKConstraint leftWingIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightWingIKConstraint;
    [SerializeField] private TwoBoneIKConstraint headIKConstraint;
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

    private Vector3 leftLegOriginPos, rightLegOriginPos, headTargetPos;
    private Quaternion leftWingRotation, rightWingRotation;
    

    //Read-only fields (to be accessed by the state machine)
    public Rig WingSpreadRig => wingSpreadRig;

    void Awake()
    {
        AssertReferences();

        if (leftLegDampConstraint == null || rightLegDampConstraint == null) return;

        leftLegOriginPos = leftLegDampConstraint.data.sourceObject.localPosition;
        rightLegOriginPos = rightLegDampConstraint.data.sourceObject.localPosition;
    }

    

    public void LookAtRotationDirection()
    {
        if (headTarget == null || neckBase == null || inputHandler == null) return;

        Vector3 neckDir = Quaternion.Euler(inputHandler.rotationInput) * Vector3.forward;
        headTargetPos = neckBase.position + (neckDir * maxHeadTargetExtendDistance);
        headTarget.position = Vector3.Lerp(headTarget.position, headTargetPos, Time.deltaTime * 4f);

        // constrain head position if its too close to neck
        headTarget.position = VectorSolver.ConstrainPosition(headTarget.position, neckBase.position, minHeadTargetExtendDistance);
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

    private void MoveWingsToTarget()
    {
        leftWingIKConstraint.data.target.position = Vector3.Lerp(leftWingIKConstraint.data.target.position, currentLeftWingTarget.position, Time.deltaTime * wingLerpSpeed);
        rightWingIKConstraint.data.target.position = Vector3.Lerp(rightWingIKConstraint.data.target.position, currentRightWingTarget.position, Time.deltaTime * wingLerpSpeed);


        currentLeftWingTarget.rotation = Quaternion.RotateTowards(currentLeftWingTarget.rotation, leftWingRotation, wingRotateSpeed * Time.deltaTime);
        currentRightWingTarget.rotation = Quaternion.RotateTowards(currentRightWingTarget.rotation, rightWingRotation, wingRotateSpeed * Time.deltaTime);
        leftWingIKConstraint.data.target.rotation = currentLeftWingTarget.rotation;
        rightWingIKConstraint.data.target.rotation = currentRightWingTarget.rotation;
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
