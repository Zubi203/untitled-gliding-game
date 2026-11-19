using UnityEngine;

public class FlapState : FlightState
{
    private float wingFlapWeight = 1f;
    private float flapDuration = 0.3f;
    private float liftForce = 10f;
    private float rotateSpeed = 10f;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    public FlapState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        targetRotation = Context.PlayerTransform.rotation;
    }
    public override void ExitState(){}
    public override void UpdateState()
    {
        base.UpdateState();
        FlapWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / flapDuration;
        float currentFlapVelocity = Mathf.Lerp(liftForce, 0f, lerpPercent);

        Vector3 targetVelocity = Vector3.up * currentFlapVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;

        Quaternion currentRotation = Quaternion.RotateTowards(startRotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
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
