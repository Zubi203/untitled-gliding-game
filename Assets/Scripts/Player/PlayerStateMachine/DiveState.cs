using UnityEngine;

public class DiveState : FlightState
{
    private float wingsCloseWeight = 0f;
    private float duration = 0.4f;
    private float diveForce = 10f;
    private Quaternion targetRotation;
    private Quaternion currentRotation;
    public DiveState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        targetRotation = Quaternion.LookRotation(-Vector3.up, Context.PlayerTransform.forward);
    }
    public override void ExitState()
    {
        Context.InputHandler.rotationInput = targetRotation.eulerAngles;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        CloseWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / duration;
        float currentDiveVelocity = Context.DiveSpeedCurve.Evaluate(lerpPercent) * diveForce;

        Vector3 targetVelocity = -Vector3.up * currentDiveVelocity;
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

    private void CloseWings()
    {
        float lerpPercent = localTime / duration;
        Context.PlayerAnimation.WingsToIdle();
        Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, wingsCloseWeight, lerpPercent);
    }
}
