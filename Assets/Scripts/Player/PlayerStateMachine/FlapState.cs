using UnityEngine;

public class FlapState : FlightState
{
    private float duration = 0.4f;
    private float liftForce = 15f;
    private float rotateSpeed = 5f;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    public FlapState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        base.EnterState();
        startRotation = Quaternion.LookRotation(Vector3.up, -Context.PlayerTransform.forward);
        targetRotation = Context.PlayerTransform.rotation;
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
        float currentFlapVelocity = Mathf.Lerp(liftForce, 0f, lerpPercent);

        Vector3 targetVelocity = Vector3.up * currentFlapVelocity;
        Context.PlayerRigidbody.linearVelocity += targetVelocity;

        Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
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
