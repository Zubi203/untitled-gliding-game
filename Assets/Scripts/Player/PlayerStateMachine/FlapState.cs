using UnityEngine;

public class FlapState : FlightState
{
    private float wingFlapWeight = 1f;
    private float flapDuration = 0.35f;
    private float liftMaxAcceleration = 6f;
    private Quaternion targetRotation;
    private Quaternion currentRotation;
    public FlapState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();

        Vector3 referenceDir = -Context.PlayerTransform.forward;
        Vector3 lookDir = Context.PlayerTransform.forward + Context.PlayerTransform.up;
        targetRotation = Quaternion.LookRotation(lookDir, referenceDir);
    }
    public override void ExitState()
    {
        Context.InputHandler.rotationInput = currentRotation.eulerAngles;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        FlapWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / flapDuration;
        float currentFlapVelocity = Context.FlapSpeedCurve.Evaluate(lerpPercent) * liftMaxAcceleration;

        Vector3 targetVelocity = Vector3.up * currentFlapVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;

        currentRotation = Quaternion.Slerp(Context.PlayerTransform.rotation, targetRotation, lerpPercent);
        Context.PlayerTransform.rotation = currentRotation;
    }
    public override FlightStateMachine.EFlightState GetNextState()
    {
        if (localTime > flapDuration)
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
        float lerpPercent = localTime / flapDuration;
        Context.PlayerAnimation.WingsToFlapEnd(lerpPercent);
        Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, wingFlapWeight, lerpPercent);
    }
}
