using UnityEngine;

public abstract class FlightState : BaseState<FlightStateMachine.EFlightState>
{
    protected FlightStateMachineContext Context;
    public FlightState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(stateKey)
    {
        Context = context;
    }
}
