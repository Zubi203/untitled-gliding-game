using UnityEngine;

public abstract class FlightState : BaseState<FlightStateMachine.EFlightState>
{
    protected float localTime = 0f;
    protected float fixedLocalTime = 0f;

    protected FlightStateMachineContext Context;
    public FlightState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(stateKey)
    {
        Context = context;
    }

    public override void EnterState()
    {
        localTime = 0f;
        fixedLocalTime = 0f;
    }

    public override void UpdateState()
    {
        localTime += Time.deltaTime;
    }

    public override void FixedUpdateState()
    {
        fixedLocalTime += Time.deltaTime;
    }
}
