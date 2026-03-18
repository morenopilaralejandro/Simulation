using Simulation.Enums.Battle;

public static class BattleArgs
{
    public static string HomeTeamId;
    public static string AwayTeamId;
    public static string HomeTeamGuid;
    public static string AwayTeamGuid;
    public static string BallId;
    public static string FieldId;
    public static BattleType BattleType;
    public static BattleResultsType BattleResultsType;
    public static WinConditionType WinConditionType;
    public static WinConditionParams WinConditionParams;

    public static void Clear()
    {
        HomeTeamId = null;
        AwayTeamId = null;
        BallId = null;
    }

    public static void Set(
        string homeTeamId = null,
        string awayTeamId = null,
        string homeTeamGuid = null,
        string awayTeamGuid = null,
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Mini,
        BattleResultsType battleResultsType = BattleResultsType.Summary,
        WinConditionType winConditionType = WinConditionType.ScoreGoals,
        WinConditionParams winConditionParams = null)
    {
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        HomeTeamGuid = homeTeamGuid;
        AwayTeamGuid = awayTeamGuid;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        BattleResultsType = battleResultsType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }

    public static void SetMini(
        string homeTeamId = null,
        string awayTeamId = null,
        string homeTeamGuid = null,
        string awayTeamGuid = null,
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Mini,
        BattleResultsType battleResultsType = BattleResultsType.Summary,
        WinConditionType winConditionType = WinConditionType.ScoreGoals,
        WinConditionParams winConditionParams = null)
    {
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        HomeTeamGuid = homeTeamGuid;
        AwayTeamGuid = awayTeamGuid;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        BattleResultsType = battleResultsType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }

    public static void SetFull(
        string homeTeamId = null,
        string awayTeamId = null,
        string homeTeamGuid = null,
        string awayTeamGuid = null,
        string ballId = "crimson",
        string fieldId = "stadium_main",
        BattleType battleType = BattleType.Full,
        BattleResultsType battleResultsType = BattleResultsType.Summary,
        WinConditionType winConditionType = WinConditionType.WinMatch,
        WinConditionParams winConditionParams = null)
    {
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        HomeTeamGuid = homeTeamGuid;
        AwayTeamGuid = awayTeamGuid;
        BallId = ballId;
        FieldId = fieldId;
        BattleType = battleType;
        BattleResultsType = battleResultsType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }
}
