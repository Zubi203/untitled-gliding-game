using UnityEngine;

public class GlideState : FlightState
{
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
            if (Context.InputHandler.inputX != 0f)
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
        float diveFactor = Vector3.Dot(Context.PlayerTransform.forward, Vector3.down);

        float horizontalForceComponentMultiplier = 0.5f * Mathf.Cos(pitchInRadians) + 1f;
        float airResistanceMultiplier = 0.5f * Mathf.Sin(pitchInRadians) + 1f;


        currentSpeed += diveFactor * Context.BaseSpeed * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, Context.MinSpeed, Context.MaxSpeed);

        Vector3 forwardVelocity = Context.PlayerTransform.forward * currentSpeed;
        
        Vector3 netVelocity = Vector3.Lerp(Context.PlayerRigidbody.linearVelocity, forwardVelocity, Context.VelocityChangeRate * Time.fixedDeltaTime);

        netVelocity += Vector3.down * Context.GravityForce * Time.fixedDeltaTime;

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
}
