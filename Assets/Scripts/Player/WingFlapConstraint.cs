using System;
using UnityEngine;

public class WingFlapConstraint : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private float maxAngleFromParent = 15f;
    private Vector3 initialLocalPosition;
    private Vector3 initialLocalRotation;

    void Awake()
    {
        if (parent == null) return;
        initialLocalPosition = transform.position - parent.position;
        initialLocalRotation = (Quaternion.Inverse(parent.rotation) * transform.rotation).eulerAngles;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(parent.rotation.eulerAngles + initialLocalRotation);
        ConstrainWingPosition();
        ConstrainWingRotation();
    }

    private void ConstrainWingRotation()
    {
        if (parent == null) return;

        Vector3 rawDirection = transform.position - parent.position;
        Vector3 constrainedDirection = VectorSolver.ClampAngleBetweenVectors(-parent.forward, rawDirection, maxAngleFromParent);
        transform.rotation = Quaternion.LookRotation(constrainedDirection);
    }
    private void ConstrainWingPosition()
    {
        if (parent == null) return;
        Vector3 targetPos = VectorSolver.ConstrainPosition(transform.position, parent.position, initialLocalPosition.magnitude);
        targetPos = targetPos.normalized * initialLocalPosition.magnitude;
        transform.position = parent.position + targetPos;
    }
}
