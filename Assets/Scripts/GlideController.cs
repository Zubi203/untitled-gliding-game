using UnityEngine;
using UnityEngine.Assertions;

public class GlideController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput inputHandler;
    [SerializeField] private float minCameraFOV = 45f;
    [SerializeField] private float maxCameraFOV = 90f;
    private Camera camera;

    [Header("Forces")]
    [SerializeField] private float tiltStrength = 500f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravityForce = 8f;

    [Header("Velocities")]
    [SerializeField] private float baseSpeed = 30f;
    [SerializeField] private float minThrustSpeed = 5f;
    [SerializeField] private float maxThrustSpeed = 400f;
    [SerializeField] private float velocityLerpFactor = 4f;
    private Rigidbody rb;
    private float currentThrust, currentTilt, tiltLerpValue;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f;
        rb.useGravity = false;
        camera = Camera.main;
        Assert.IsNotNull(inputHandler, "PlayerInput has not been assigned to the Glide Controller");
    }

    private void FixedUpdate()
    {
        GlideMovement();
    }

    private void Update()
    {
        GlideRotation();
        AdjustCameraFOV();
    }

    private void GlideMovement()
    {
        if (rb == null) return;

        float pitchInRadians = inputHandler.transform.eulerAngles.x * Mathf.Deg2Rad;
        float diveFactor = Vector3.Dot(transform.forward, Vector3.down);

        float horizontalForceComponentMultiplier = 0.5f * Mathf.Cos(pitchInRadians) + 1f;
        float airResistanceMultiplier = 0.5f * Mathf.Sin(pitchInRadians) + 1f;


        currentThrust += diveFactor * baseSpeed * Time.fixedDeltaTime;
        currentThrust = Mathf.Clamp(currentThrust, minThrustSpeed, maxThrustSpeed);

        Vector3 forwardVelocity = transform.forward * currentThrust;
        
        Vector3 netVelocity = Vector3.Lerp(rb.linearVelocity, forwardVelocity, velocityLerpFactor * Time.fixedDeltaTime);

        netVelocity += Vector3.down * gravityForce * Time.fixedDeltaTime;

        rb.linearVelocity = netVelocity;

        
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

        Quaternion targetRotation = Quaternion.Euler(inputHandler.transform.eulerAngles.x, inputHandler.transform.eulerAngles.y, -currentTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void AdjustCameraFOV()
    {
        if (rb == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = currentSpeed / maxThrustSpeed;
        float cameraFOVRange = maxCameraFOV - minCameraFOV;
        float cameraFOV = minCameraFOV + (cameraFOVRange * currentSpeedPercent);
        camera.fieldOfView = cameraFOV;

    }
}
