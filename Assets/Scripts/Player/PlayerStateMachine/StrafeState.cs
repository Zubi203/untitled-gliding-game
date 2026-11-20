using UnityEngine;

public class StrafeState : FlightState
{
    private float wingFlapWeight = 1f;
    private float duration = 0.4f;
    private float maxStrafeVelocity = 7f;
    private float minStrafeVelocity = 0f;
    private float currentStrafeVelocity = 0f;
    private Vector3 strafeDir;
    private Quaternion targetRotation;
    private Quaternion currentRotation;

    public StrafeState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        strafeDir = Context.PlayerTransform.right * Context.InputHandler.inputX;
        strafeDir = new Vector3(strafeDir.x, 0f, strafeDir.z);
        targetRotation = Quaternion.LookRotation(strafeDir, -Context.PlayerTransform.forward);
        
    }
    public override void ExitState()
    {
        Context.InputHandler.rotationInput = targetRotation.eulerAngles;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        FlapWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / duration;
        currentStrafeVelocity = (Context.StrafeSpeedCurve.Evaluate(lerpPercent) * maxStrafeVelocity) + minStrafeVelocity;

        Vector3 targetVelocity = strafeDir.normalized * currentStrafeVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;

        currentRotation = Quaternion.Slerp(Context.PlayerTransform.rotation, targetRotation, lerpPercent);
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

    private void FlapWings()
    {
        if (localTime < 0.25f * duration)
        {
            float lerpPercent = localTime / (0.25f * duration);
            Context.PlayerAnimation.WingsToFlapStart(lerpPercent);
            Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, wingFlapWeight, lerpPercent);
        }
        else
        {
            float lerpPercent = (localTime - (0.25f * duration)) / (0.75f * duration);
            Context.PlayerAnimation.WingsToFlapEnd(lerpPercent);
            Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, wingFlapWeight, lerpPercent);
        }
        
    }
}
