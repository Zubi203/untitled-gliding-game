using UnityEngine;

public static class VectorSolver
{
    public static Vector3 ClampAngleBetweenVectors(Vector3 fromDir, Vector3 toDir, float maxAngle)
    {
        Quaternion rotationDiff = Quaternion.FromToRotation(fromDir, toDir);
        rotationDiff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > maxAngle)
        {
            return Quaternion.AngleAxis(maxAngle, axis) * fromDir;
        }

        return toDir;
    }

    public static Vector3 ConstrainPosition(Vector3 objectPosToConstrain, Vector3 sourceObjectPos, float maxConstrainedDistance)
    {
        float distance = Vector3.Distance(objectPosToConstrain, sourceObjectPos);
        if (distance > maxConstrainedDistance || distance < maxConstrainedDistance)
        {
            Vector3 dirFromPrevious = objectPosToConstrain - sourceObjectPos;
            return sourceObjectPos + (dirFromPrevious.normalized * maxConstrainedDistance);
        }

        return objectPosToConstrain;
    }
}
