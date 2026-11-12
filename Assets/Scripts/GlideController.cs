using UnityEngine;

public class GlideController : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 30f;
    [SerializeField] private float minThrustSpeed = 5f;
    [SerializeField] private float maxThrustSpeed = 400f;
    [SerializeField] private float tiltStrength = 500f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float dragFactor = 1f;
    [SerializeField] private float minDrag = 0.2f;
    [SerializeField] private float minAcceleration = 0.5f;
    [SerializeField] private float maxAcceleration = 1f;
    private Rigidbody rb;
    private Transform cameraAnchor;
    private float currentThrust;
    private float currentTilt;
    private float tiltLerpValue;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraAnchor = Camera.main.transform.parent;
    }

    private void FixedUpdate()
    {
        GlideMovement();
    }

    private void Update()
    {
        GlideRotation();
    }

    private void GlideMovement()
    {
        if (rb == null) return;

        float pitchInDegrees = cameraAnchor.eulerAngles.x % 360f;

        float pitchInRadians = cameraAnchor.eulerAngles.x * Mathf.Deg2Rad;
        float mappedPitch = Mathf.Sin(pitchInRadians) * baseSpeed;
        float offsetMappedPitch = Mathf.Cos(pitchInRadians) * dragFactor;

        //tweak acceleration based on pitch angle
        float accelerationPercent = (pitchInDegrees >= 300f) ? minAcceleration : maxAcceleration;

        currentThrust += mappedPitch * accelerationPercent * Time.fixedDeltaTime;
        currentThrust = Mathf.Clamp(currentThrust, 0f, maxThrustSpeed);

        if (rb.linearVelocity.magnitude > minThrustSpeed)
        {
            rb.AddRelativeForce(Vector3.forward * currentThrust);
            rb.linearDamping = Mathf.Clamp(offsetMappedPitch, minDrag, dragFactor);
        }
        else
        {
            currentThrust = 0f;
        }
        
    }

    private void GlideRotation()
    {
        float inputX = Input.GetAxis("Mouse X");
        currentTilt += inputX * tiltStrength;

        if (inputX == 0f)
        {
            currentTilt = Mathf.Lerp(currentTilt, 0f, tiltLerpValue);
            tiltLerpValue += Time.deltaTime;
        }
        else
        {
            tiltLerpValue = 0f;
        }

        Quaternion targetRotation = Quaternion.Euler(cameraAnchor.eulerAngles.x, cameraAnchor.eulerAngles.y, -currentTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
