using UnityEngine;

public class DiveState : FlightState
{
    public DiveState(FlightStateMachineContext context, FlightStateMachine.EFlightState stateKey) : base(context , stateKey)
    {
        Context = context;
    }

    public override void EnterState(){}
    public override void ExitState(){}
    public override void UpdateState(){}
    public override void FixedUpdateState(){}
    public override FlightStateMachine.EFlightState GetNextState()
    {
        return StateKey;
    }
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerStay(Collider other){}
    public override void OnTriggerExit(Collider other){}
}
