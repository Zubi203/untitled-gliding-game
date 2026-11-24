using UnityEngine;

public class StrafeState : FlightState
{
    private float duration = 0.6f;
    private float maxStrafeVelocity = 12f; 
    private float minStrafeVelocity = 0f;
    private float currentStrafeVelocity = 0f;
    private Vector3 strafeDir;
    private Quaternion startingRotation;
    private Vector3 rotationAxis;

    public StrafeState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        strafeDir = Context.PlayerTransform.right * Context.InputHandler.inputX;
        strafeDir = new Vector3(strafeDir.x, 0f, strafeDir.z);

        startingRotation = Context.PlayerTransform.localRotation;
        rotationAxis = (-Vector3.forward * Context.InputHandler.inputX).normalized;
    }
    public override void ExitState(){}
    public override void UpdateState()
    {
        base.UpdateState();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / duration;
        currentStrafeVelocity = (Context.StrafeSpeedCurve.Evaluate(lerpPercent) * maxStrafeVelocity) + minStrafeVelocity;

        Vector3 targetVelocity = strafeDir.normalized * currentStrafeVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;
        Context.PlayerRigidbody.linearVelocity = Vector3.ClampMagnitude(Context.PlayerRigidbody.linearVelocity, Context.MaxSpeed);

        float currentAngleLerpPercent = Context.StrafeRotationCurve.Evaluate(lerpPercent);
        float currentAngle = Mathf.Lerp(0f, 420f, currentAngleLerpPercent);
        Context.PlayerTransform.localRotation = startingRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);

    }
    public override FlightStateMachine.EFlightState GetNextState()
    {
        if (localTime >= duration)
        {
            return FlightStateMachine.EFlightState.Glide;
        }
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}

}
