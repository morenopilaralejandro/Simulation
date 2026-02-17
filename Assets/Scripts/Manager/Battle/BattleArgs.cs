using Simulation.Enums.Battle;

public static class BattleArgs
{
    public static string TeamId0;
    public static string TeamId1;
    public static string BallId;
    public static string FieldId;
    public static BattleType BattleType;
    public static WinConditionType WinConditionType;
    public static WinConditionParams WinConditionParams;

    public static void Clear()
    {
        TeamId0 = null;
        TeamId1 = null;
        BallId = null;
    }

    public static void Set(
        string teamId0, 
        string teamId1, 
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Mini,
        WinConditionType winConditionType = WinConditionType.ScoreGoals,
        WinConditionParams winConditionParams = null)
    {
        TeamId0 = teamId0;
        TeamId1 = teamId1;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }

    public static void SetMini(
        string teamId0, 
        string teamId1, 
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Mini,
        WinConditionType winConditionType = WinConditionType.ScoreGoals,
        WinConditionParams winConditionParams = null)
    {
        TeamId0 = teamId0;
        TeamId1 = teamId1;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }

    public static void SetFull(
        string teamId0, 
        string teamId1, 
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Full,
        WinConditionType winConditionType = WinConditionType.WinMatch,
        WinConditionParams winConditionParams = null)
    {
        TeamId0 = teamId0;
        TeamId1 = teamId1;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }
}
