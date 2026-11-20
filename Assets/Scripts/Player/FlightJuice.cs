using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlightJuice : MonoBehaviour
{
    public float maxPlayerSpeed = 200f;
    [SerializeField] private Volume postProcessing;

    [Header("Camera")]
    [SerializeField] private float minCameraFOV = 45f;
    [SerializeField] private float maxCameraFOV = 80f;
    [SerializeField] private float minCameraZOffset = -15f;
    [SerializeField] private float maxCameraZOffset = -5f;
    [SerializeField] private float FOVLerpSpeed = 5f;

    [Header("Chromatic Aberration")]
    [SerializeField] private float minChromaticAbberationIntensity = 0f;
    [SerializeField] private float maxChromaticAbberationIntensity = 0.5f;

    [Header("Depth of Field")]
    [SerializeField] private float minDepthOfFieldAperture = 1f;
    [SerializeField] private float maxDepthOfFieldAperture = 32f;

    private Camera camera;
    private Rigidbody rb;
    private float currentCameraFOV, currentCameraOffset, currentChromaticAberration, currentDepthOfField;
    private DepthOfField _depthOfField;
    private ChromaticAberration _chromaticAberration;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;

        if (postProcessing == null) return;

        if (postProcessing.profile.TryGet(out DepthOfField dof))
        {
            _depthOfField = dof;
        }

        if (postProcessing.profile.TryGet(out ChromaticAberration ca))
        {
            _chromaticAberration = ca;
        }
    }

    void Update()
    {
        ApplyCameraEffects();
        AdjustChromaticAberration();
        AdjustDepthOfField();

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, currentCameraFOV, Time.deltaTime * FOVLerpSpeed);

        Vector3 targetCameraOffset = new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y, currentCameraOffset);
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, targetCameraOffset, Time.deltaTime * FOVLerpSpeed);
    }

    public void ApplyCameraEffects()
    {
        AdjustCameraFOV();
        AdjustCameraPosition();
        
    }

    void AdjustCameraFOV()
    {
        if (rb == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = currentSpeed / maxPlayerSpeed;
        float cameraFOVRange = maxCameraFOV - minCameraFOV;
        currentCameraFOV = minCameraFOV + (cameraFOVRange * currentSpeedPercent);
        

    }

    void AdjustCameraPosition()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = 1f - (currentSpeed / maxPlayerSpeed);
        float cameraOffsetRange = maxCameraZOffset - minCameraZOffset;
        currentCameraOffset = maxCameraZOffset - (cameraOffsetRange * currentSpeedPercent);
        
    }

    void AdjustChromaticAberration()
    {
        if (_chromaticAberration == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = currentSpeed / maxPlayerSpeed;
        float chromaticAberrationRange = maxChromaticAbberationIntensity - minChromaticAbberationIntensity;
        currentChromaticAberration = minChromaticAbberationIntensity + (chromaticAberrationRange * currentSpeedPercent);
        _chromaticAberration.intensity.value = currentChromaticAberration;
    }

    void AdjustDepthOfField()
    {
        if (_depthOfField == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        float currentSpeedPercent = currentSpeed / maxPlayerSpeed;
        float depthOfFieldRange = maxDepthOfFieldAperture - minDepthOfFieldAperture;
        currentDepthOfField = maxDepthOfFieldAperture - (depthOfFieldRange * currentSpeedPercent);
        _depthOfField.aperture.value = currentDepthOfField;
    }
}
