using UnityEngine;

public class FlapStartState : FlightState
{
    private float wingRaiseWeight = 1f;
    private float duration = 0.15f;
    private float dipVelocity = 10f;
    private float forwardVelocityReductionPercent = 0.7f;
    private Vector3 startingVelocity;
    private Vector3 targetVelocity;
    public FlapStartState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        Context.PlayerRigidbody.linearVelocity *= forwardVelocityReductionPercent;
        startingVelocity = Context.PlayerRigidbody.linearVelocity;
        targetVelocity = startingVelocity * forwardVelocityReductionPercent;
    }
    public override void ExitState(){}
    public override void UpdateState()
    {
        base.UpdateState();
        RaiseWings();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        float lerpPercent = localTime / duration;
        float currentDipVelocity = Mathf.Lerp(dipVelocity, 0f, lerpPercent);

        Vector3 currentVelocity = Vector3.Lerp(startingVelocity, targetVelocity, lerpPercent);
        currentVelocity += -Vector3.up * currentDipVelocity;
        Context.PlayerRigidbody.linearVelocity = currentVelocity;
    }
    public override FlightStateMachine.EFlightState GetNextState()
    {
        if (localTime >= duration)
        {
            return FlightStateMachine.EFlightState.Flap;
        }
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}

    private void RaiseWings()
    {
        float lerpPercent = localTime / duration;
        Context.PlayerAnimation.WingsToFlapStart(lerpPercent);
        Context.PlayerAnimation.WingSpreadRig.weight = Mathf.Lerp(Context.PlayerAnimation.WingSpreadRig.weight, wingRaiseWeight, lerpPercent);
    }
}
