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
        Transitioning,
        InEncounter
    }

    public enum PlayerWorldState
    {
        FreeRoam,
        InDialogue,
        InMenu,
        InCutscene,
        InBattle,
        Transitioning,
        Paused
    }

    // FacingDirection.cs
    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}

