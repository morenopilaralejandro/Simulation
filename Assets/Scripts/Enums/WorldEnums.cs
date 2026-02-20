namespace Simulation.Enums.World
{
    public enum SpawnPointType
    {
        Default,
        DoorEntry,
        DoorExit,
        WorldTransition,
        BattleReturn,
        Checkpoint
    }

    public enum ZoneType
    {
        Overworld,
        Interior
    }

    public enum WorldState
    {
        None,
        Loading,
        InOverworld,
        InInterior,
        Transitioning
    }
}

