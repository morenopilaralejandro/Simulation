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

    public enum Realm
    {
        Earth,
        Heaven,
        Saturn
    }

    public enum WorldState
    {
        None,
        Loading,
        Unloaded,
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

    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum ChestState
    {
        Closed,
        Locked,
        Opened
    }
}

