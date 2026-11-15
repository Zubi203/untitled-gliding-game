using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class FlightStateMachine : StateMachine<FlightStateMachine.EFlightState>
{
    public enum EFlightState
    {
        Glide,
        Strafe,
        FlapStart,
        Flap,
        Dive
    }

    [SerializeField] private FlightStateMachineContext context;

    void Awake()
    {
        AssertReferences();
        InitializeStates();
    }

    void AssertReferences()
    {
        Assert.IsNotNull(context, "State machine context has not been assigned");
    }

    void InitializeStates()
    {
        States.Add(EFlightState.Glide, new GlideState(context, EFlightState.Glide));
        States.Add(EFlightState.Strafe, new StrafeState(context, EFlightState.Strafe));
        States.Add(EFlightState.FlapStart, new FlapStartState(context, EFlightState.FlapStart));
        States.Add(EFlightState.Flap, new FlapState(context, EFlightState.Flap));
        States.Add(EFlightState.Dive, new DiveState(context, EFlightState.Dive));

        currentState = States[EFlightState.Glide];
    }
}
