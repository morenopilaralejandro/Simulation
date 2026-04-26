using UnityEngine;
using Aremoreno.Enums.World;

public class ChestComponentStateMachine
{
    public ChestState State { get; private set; }

    public bool IsOpened => State == ChestState.Opened;
    public bool IsLocked => State == ChestState.Locked;

    public ChestComponentStateMachine()
    {
        Initialize();
    }

    public void Initialize()
    {
        State = ChestState.Closed;
    }

    public void SetState(ChestState newState) 
    {
        State = newState;
    }
}
