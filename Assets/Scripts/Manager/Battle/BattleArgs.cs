using Aremoreno.Enums.Battle;
using Aremoreno.Enums.World;

public static class BattleArgs
{
    public static string HomeTeamId;
    public static string AwayTeamId;
    public static string HomeTeamGuid;
    public static string AwayTeamGuid;
    public static string BallId;
    public static string FieldId;
    public static string BgmId;
    public static int HomeTeamLevel;
    public static int AwayTeamLevel;
    public static bool HomeTeamCanUseWing = true;
    public static bool AwayTeamCanUseWing = true;
    public static TimeOfDay TimeOfDay;
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
        string ballId = "ball-00001-default",
        string fieldId = "field-00001-stadium_main",
        string bgmId = "bgm-battle",
        TimeOfDay timeOfDay = TimeOfDay.Day,
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
        BgmId = bgmId;
        TimeOfDay = timeOfDay;
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
        string ballId = "ball-00001-default",
        string fieldId = "field-00001-stadium_main",
        string bgmId = "bgm-battle",
        TimeOfDay timeOfDay = TimeOfDay.Day,
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
        BgmId = bgmId;
        TimeOfDay = timeOfDay;
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
        string ballId = "ball-00001-default",
        string fieldId = "field-00001-stadium_main",
        string bgmId = "bgm-battle",
        TimeOfDay timeOfDay = TimeOfDay.Day,
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
        BgmId = bgmId;
        TimeOfDay = timeOfDay;
        BattleType = battleType;
        BattleResultsType = battleResultsType;
        WinConditionType = winConditionType;
        WinConditionParams = winConditionParams ?? new WinConditionParams();
    }
}
