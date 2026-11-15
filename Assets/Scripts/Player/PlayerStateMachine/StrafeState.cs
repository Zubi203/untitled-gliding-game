using UnityEngine;

public class StrafeState : FlightState
{
    private float duration = 0.5f;
    private float maxStrafeVelocity = 10f;
    private float minStrafeVelocity = 0f;
    private float currentStrafeVelocity = 0f;
    private float startingRotation = 0f;
    private float targetRotation = 0f;
    private Vector3 strafeDir;

    public StrafeState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        startingRotation = Context.PlayerTransform.rotation.eulerAngles.z;
        targetRotation = startingRotation + 360f * -Context.InputHandler.inputX;

        strafeDir = Context.PlayerTransform.right * Context.InputHandler.inputX;
        strafeDir = new Vector3(strafeDir.x, 0f, strafeDir.z);
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
        currentStrafeVelocity = Mathf.Lerp(maxStrafeVelocity, minStrafeVelocity, lerpPercent);

        Vector3 targetVelocity = strafeDir.normalized * currentStrafeVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;

        float currentRotationZ = Mathf.Lerp(startingRotation, targetRotation, lerpPercent);
        Quaternion currentRotation = Quaternion.Euler(Context.PlayerTransform.eulerAngles.x, Context.PlayerTransform.eulerAngles.y, currentRotationZ);
        Context.PlayerTransform.rotation = currentRotation;

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
