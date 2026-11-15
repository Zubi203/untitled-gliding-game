using UnityEngine;

public class FlightJuice : MonoBehaviour
{
    public float maxPlayerSpeed = 200f;
    [SerializeField] private float minCameraFOV = 45f;
    [SerializeField] private float maxCameraFOV = 80f;
    private Camera camera;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
    }

    void Update()
    {
        AdjustCameraFOV();
    }

    void AdjustCameraFOV()
    {
        if (rb == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = currentSpeed / maxPlayerSpeed;
        float cameraFOVRange = maxCameraFOV - minCameraFOV;
        float cameraFOV = minCameraFOV + (cameraFOVRange * currentSpeedPercent);
        camera.fieldOfView = cameraFOV;

    }
}
