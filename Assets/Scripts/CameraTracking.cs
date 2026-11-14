using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothing = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxCameraSmoothingRange = 30f;
    [SerializeField] private Vector3 offset;

    private Vector3 currentVelocity;

    void FixedUpdate()
    {
        FollowTargetTransform();
    }
    
    void Update()
    {
        RotateCamera();
    }

    void FollowTargetTransform()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        float distanceToTarget = Vector3.Distance(transform.position, targetPos);

        if (distanceToTarget < maxCameraSmoothingRange)
        {
            Vector3 interpolatedPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothing);
            transform.position = interpolatedPos;
        }
        else
        {
            transform.position = targetPos;
        }
    }

    void RotateCamera()
    {
        if (target == null) return;

        Quaternion targetRotation = Quaternion.Euler(target.eulerAngles.x, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
