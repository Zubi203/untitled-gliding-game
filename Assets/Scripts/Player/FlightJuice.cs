using UnityEngine;

public class FlightJuice : MonoBehaviour
{
    public float maxPlayerSpeed = 200f;
    [SerializeField] private float minCameraFOV = 45f;
    [SerializeField] private float maxCameraFOV = 80f;
    [SerializeField] private float FOVLerpSpeed = 5f;
    private Camera camera;
    private Rigidbody rb;
    private float currentCameraFOV;

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
        currentCameraFOV = minCameraFOV + (cameraFOVRange * currentSpeedPercent);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, currentCameraFOV, Time.deltaTime * FOVLerpSpeed);

    }
}
