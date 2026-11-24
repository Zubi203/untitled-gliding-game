using UnityEngine;
using UnityEngine.Assertions;

public class FlightStateMachineContext: MonoBehaviour
{

    [Header("Speed Curves")]
    [SerializeField] private AnimationCurve flapSpeedCurve;
    [SerializeField] private AnimationCurve strafeSpeedCurve;
    [SerializeField] private AnimationCurve strafeRotationCurve;
    [SerializeField] private AnimationCurve diveSpeedCurve;

    [Header("Forces")]
    [SerializeField] private float maxTilt = 50f;
    [SerializeField] private float tiltStrength = 500f;
    [SerializeField] private float maxRotationChangeRate = 8f;
    [SerializeField] private float minRotationChangeRate = 2f;
    [SerializeField] private float gravityForce = 8f;

    [Header("Velocities")]
    [SerializeField] private float glideMaxAcceleration = 30f;
    [SerializeField] private float minSpeed = 30f;
    [SerializeField] private float maxSpeed = 400f;
    [SerializeField] private float velocityChangeRate = 4f;

    [Header("Object References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private PlayerInput inputHandler;
    [SerializeField] private FlightJuice flightJuice;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerProceduralAnimation playerAnimation;

    void Awake()
    {
        AssertReferences();

        flightJuice.maxPlayerSpeed = maxSpeed;
        playerRigidbody.useGravity = false;
        playerRigidbody.linearDamping = 0;
    }

    void AssertReferences()
    {
        Assert.IsNotNull(playerRigidbody, "Player Rigidbody has not been assigned");
        Assert.IsNotNull(inputHandler, "Player Input has not been assigned");
        Assert.IsNotNull(flightJuice, "Flight Juice has not been assigned");
        Assert.IsNotNull(playerTransform, "Player Transform has not been assigned");
        Assert.IsNotNull(playerAnimation, "Player Animation has not been assigned");
    }

    // Read-only fields
    public AnimationCurve FlapSpeedCurve => flapSpeedCurve;
    public AnimationCurve DiveSpeedCurve => diveSpeedCurve;
    public AnimationCurve StrafeSpeedCurve => strafeSpeedCurve;
    public AnimationCurve StrafeRotationCurve => strafeRotationCurve;
    public Rigidbody PlayerRigidbody => playerRigidbody;
    public PlayerInput InputHandler => inputHandler;
    public FlightJuice FlightJuiceComponent => flightJuice;
    public PlayerProceduralAnimation PlayerAnimation => playerAnimation;
    public Transform PlayerTransform => playerTransform;
    public float TiltStrength => tiltStrength;
    public float MaxTilt => maxTilt;
    public float MaxRotationChangeRate => maxRotationChangeRate;
    public float MinRotationChangeRate => minRotationChangeRate;
    public float GravityForce => gravityForce;
    public float MinSpeed => minSpeed;
    public float MaxSpeed => maxSpeed;
    public float GlideMaxAcceleration => glideMaxAcceleration;
    public float VelocityChangeRate => velocityChangeRate;
    
}
