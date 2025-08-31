using Simulation.Enums.Battle;

public static class BattleArgs
{
    public static string TeamId0;
    public static string TeamId1;
    public static BattleType BattleType;

    public static void Clear()
    {
        TeamId0 = null;
        TeamId1 = null;
    }
}
