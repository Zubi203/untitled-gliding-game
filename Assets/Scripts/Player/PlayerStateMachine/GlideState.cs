using UnityEngine;

public class GlideState : FlightState
{
    private float maxWingSpreadWeight = 1f;
    private float minWingSpreadWeight = 0f;
    private float wingSpreadSpeed = 5f;
    private float currentSpeed, currentTilt, tiltLerpValue;

    public GlideState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        
    }
    public override void ExitState(){}
    public override void UpdateState()
    {
        base.UpdateState();
        GlideRotation();
        Context.PlayerAnimation.LookAtRotationDirection();
        SpreadWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        GlideMovement();
    }
    public override FlightStateMachine.EFlightState GetNextState()
    {
        if (Context.InputHandler.specialActionInput)
        {
            if (Mathf.Abs(Context.InputHandler.inputX) > Mathf.Abs(Context.InputHandler.inputY) && Context.InputHandler.inputX != 0f)
            {
                return FlightStateMachine.EFlightState.Strafe;
            }

            if (Context.InputHandler.inputY > 0f)
            {
                return FlightStateMachine.EFlightState.Dive;
            }

            return FlightStateMachine.EFlightState.FlapStart;
        }

        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}

    private void GlideMovement()
    {
        float pitchInRadians = Context.InputHandler.rotationInput.x * Mathf.Deg2Rad;
        float accelerationVectorMultiplier = Vector3.Dot(Context.PlayerTransform.forward, Vector3.down);
        float horizontalForceMultiplier = 1f - Mathf.Abs(accelerationVectorMultiplier);

        Vector3 baseForwardForce = Context.MinSpeed * Context.PlayerTransform.forward;

        currentSpeed += accelerationVectorMultiplier * Context.GlideMaxAcceleration * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -Context.MaxSpeed, Context.MaxSpeed);
        Vector3 accelerationVector = Context.PlayerTransform.forward * currentSpeed;
        Vector3 forwardVelocity = baseForwardForce + accelerationVector;

        Vector3 horizontalComponent = new Vector3(forwardVelocity.x, 0f, forwardVelocity.z);
        Vector3 absoluteHorizontalVelocity = new Vector3(Context.PlayerTransform.forward.x, 0f, Context.PlayerTransform.forward.z) * Mathf.Abs(horizontalComponent.magnitude);
        forwardVelocity = new Vector3(absoluteHorizontalVelocity.x, forwardVelocity.y, absoluteHorizontalVelocity.z);
        
        Vector3 netVelocity = Vector3.Lerp(Context.PlayerRigidbody.linearVelocity, forwardVelocity, Context.VelocityChangeRate * Time.fixedDeltaTime);

        netVelocity += Vector3.down * Context.GravityForce * Time.fixedDeltaTime;

        netVelocity = Vector3.ClampMagnitude(netVelocity, Context.MaxSpeed);

        Context.PlayerRigidbody.linearVelocity = netVelocity;
    }

    private void GlideRotation()
    {
        float inputX = Context.InputHandler.inputX;
        currentTilt += inputX * Context.TiltStrength;
        currentTilt = Mathf.Clamp(currentTilt, -Context.MaxTilt, Context.MaxTilt);

        if (inputX == 0f)
        {
            currentTilt = Mathf.Lerp(currentTilt, 0f, tiltLerpValue);
            tiltLerpValue += Time.deltaTime;
        }
        else
        {
            tiltLerpValue = 0f;
        }

        Quaternion targetRotation = Quaternion.Euler(Context.InputHandler.rotationInput.x, Context.InputHandler.rotationInput.y, -currentTilt);
        Context.PlayerTransform.rotation = Quaternion.Lerp(Context.PlayerTransform.rotation, targetRotation, Context.RotationChangeRate * Time.deltaTime);
    }

    private void SpreadWings()
    {
        Context.PlayerAnimation.WingsToIdle();

        float weightRange = maxWingSpreadWeight - minWingSpreadWeight;
        float pitchFactor = Vector3.Dot(Context.PlayerTransform.forward, Vector3.down);
        pitchFactor = 1f - pitchFactor;
        float targetWeight = (weightRange * pitchFactor) + minWingSpreadWeight;
        Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, targetWeight, wingSpreadSpeed * Time.deltaTime);
    }

}
