using UnityEngine;
using Simulation.Enums.World;

public class ChestComponentStateMachine
{
    public ChestState State { get; private set; }

    public bool IsOpened => State == ChestState.Opened;
    public bool IsLocked => State == ChestState.Locked;

    public ChestComponentStateMachine(ChestState state)
    {
        Initialize(state);
    }

    public void Initialize(ChestState state)
    {
        State = state;
    }

    public void SetState(ChestState newState) 
    {
        State = newState;
    }
}
