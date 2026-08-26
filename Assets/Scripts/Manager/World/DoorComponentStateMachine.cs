using UnityEngine;
using Aremoreno.Enums.World;

public class DoorComponentStateMachine
{
    public DoorState State { get; private set; }

    public bool IsOpened => State == DoorState.Opened;
    public bool IsLocked => State == DoorState.Locked;

    public DoorComponentStateMachine()
    {
        Initialize();
    }

    public void Initialize()
    {
        State = DoorState.Locked;
    }

    public void SetState(DoorState newState) 
    {
        State = newState;
    }
}
