using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Log;

public class DuelLogManager : MonoBehaviour
{
    #region Fields

    public static DuelLogManager Instance { get; private set; }

    public List<DuelLogEntry> DuelLogEntries = new List<DuelLogEntry>();
    public Color ColorHome { get; private set; }
    public Color ColorAway { get; private set; }
    public Color ColorDefault { get; private set; }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() 
    {
        ColorHome = ColorManager.GetTeamIndicatorColor(TeamSide.Home, false);
        ColorAway = ColorManager.GetTeamIndicatorColor(TeamSide.Away, false);
        ColorDefault = Color.black;
    }


    void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        BattleEvents.OnBattleEnd += HandleBattleEnd;
        BattleEvents.OnBattlePause += HandleBattlePause;
        BattleEvents.OnBattleResume += HandleBattleResume;

        BattleEvents.OnPassPerformed += HandlePassPerformed;
        BattleEvents.OnShootPerformed += HandleShootPerformed;
        BattleEvents.OnGoalScored += HandleGoalScored;
        BattleEvents.OnShootStopped += HandleShootStopped;

        DuelEvents.OnDuelStart += HandleDuelStart;
        DuelEvents.OnDuelEnd += HandleDuelEnd;
        DuelEvents.OnDuelCancel += HandleDuelCancel;
        BallEvents.OnGained += HandleGained;
    }

    void OnDisable()
    { 
        BattleEvents.OnBattleStart -= HandleBattleStart;
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
        BattleEvents.OnBattlePause -= HandleBattlePause;
        BattleEvents.OnBattleResume -= HandleBattleResume;

        BattleEvents.OnPassPerformed -= HandlePassPerformed;
        BattleEvents.OnShootPerformed -= HandleShootPerformed;
        BattleEvents.OnGoalScored -= HandleGoalScored;
        BattleEvents.OnShootStopped -= HandleShootStopped;

        DuelEvents.OnDuelStart -= HandleDuelStart;
        DuelEvents.OnDuelEnd -= HandleDuelEnd;
        DuelEvents.OnDuelCancel -= HandleDuelCancel;
        BallEvents.OnGained -= HandleGained;
    }

    private void HandleBattleStart(BattleType battleType) 
    { 
        Clear();
        DuelLogManager.Instance.AddMatchStart();
    }
    private void HandleBattleEnd() => AddMatchEnd();
    private void HandleBattlePause(TeamSide teamSide) => AddMatchPause(GoalManager.Instance.Keepers[teamSide]);
    private void HandleBattleResume() => AddMatchResume();

    private void HandlePassPerformed(CharacterEntityBattle character) => AddActionPass(character.Character, character.TeamSide);
    private void HandleShootPerformed(CharacterEntityBattle character, bool isDirect) 
    {
        AddActionShoot(character.Character, character.TeamSide);
        if (isDirect) AddActionDirect(character.Character, character.TeamSide);
    }
    private void HandleGoalScored(CharacterEntityBattle scorringCharacter) => AddActionScore(scorringCharacter.Character, scorringCharacter.TeamSide);
    private void HandleShootStopped(CharacterEntityBattle character) => AddActionStop(character.Character, character.TeamSide);

    private void HandleDuelStart(DuelMode duelMode) => AddDuelStart();
    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser)
    {
        if (isWinnerUser)
            AddDuelWin(winner.CharacterEntityBattle.Character, winner.CharacterEntityBattle.TeamSide);
        else
            AddDuelLose(winner.CharacterEntityBattle.Character, winner.CharacterEntityBattle.TeamSide);
    }
    private void HandleDuelCancel(DuelMode duelMode) => AddDuelCancel();
    private void HandleGained(CharacterEntityBattle character) => AddPossessionGained(character.Character, character.TeamSide);
        



    #endregion

    #region Add Methods

    private void AddEntry(string entryId, LogLevel logLevel, Character character, TeamSide teamSide, Move move, object args) 
    {
        DuelLogEntry entry = new DuelLogEntry(entryId, logLevel, character, teamSide, move, args);
        DuelLogEntries.Add(entry);
        LogManager.Trace($"[DuelLogManager] Shown: {logLevel == LogLevel.Info} String: {entry.EntryString}");
        DuelLogEvents.RaiseNewEntry(entry);
    }

    public void AddMatchStart()
    {
        AddEntry(
            "match_start", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddMatchHalf()
    {
        AddEntry(
            "match_half", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddMatchEnd()
    {
        AddEntry(
            "match_end", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddMatchPause(CharacterEntityBattle character)
    {
        var args = new { 
            teamName = character.GetTeam().TeamName
        };
        AddEntry(
            "match_pause", 
            LogLevel.Trace,
            character.Character,
            character.TeamSide,
            null,
            args
        );
    }

    public void AddMatchResume()
    {
        AddEntry(
            "match_resume", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddDeadBallKickoff(Character character, TeamSide teamSide)
    {
        if(character.PortraitSprite == null) return;
        AddEntry(
            "deadball_kickoff", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDeadBallThrowIn(Character character, TeamSide teamSide)
    {
        AddEntry(
            "deadball_throw_in", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDeadBallCornerKick(Character character, TeamSide teamSide)
    {
        AddEntry(
            "deadball_corner_kick", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDeadBallGoalKick(Character character, TeamSide teamSide)
    {
        AddEntry(
            "deadball_goal_kick", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDeadBallFreeKickDirect(Character character, TeamSide teamSide)
    {
        AddEntry(
            "deadball_free_kick_direct", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDeadBallFreeKickIndirect(Character character, TeamSide teamSide)
    {
        AddEntry(
            "deadball_free_kick_indirect", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddActionPass(Character character, TeamSide teamSide)
    {
        if (DeadBallManager.Instance.IsFirstKickoff) return;

        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_pass", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddActionShoot(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_shoot", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddActionDirect(Character character, TeamSide teamSide)
    {
        AddEntry(
            "action_direct", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddActionCommand(Character character, TeamSide teamSide, DuelCommand command, Move move)
    {
        string characterName = character.CharacterNick;
        string commandName;

        if (move != null)
        {
            commandName = move.MoveName;
        }
        else
        {
            commandName = command == DuelCommand.Melee
                ? new LocalizedString("UI-Battle-Localized", "button_command_melee").GetLocalizedString()
                : new LocalizedString("UI-Battle-Localized", "button_command_ranged").GetLocalizedString();
        }

        var argsLong = new { 
            characterName = characterName,
            commandName = commandName
        };
        var argsShort = new { 
            commandName = commandName
        };

        AddEntry(
            "action_command", 
            LogLevel.Trace,
            character,
            teamSide,
            move,
            argsLong
        );
    }

    public void AddActionDamage(Character character, TeamSide teamSide, DuelAction action, float damage)
    {
        string actionSymbol = (action == DuelAction.Offense)
            ? "+"
            : "-";

        string damageNumber = Mathf.RoundToInt(damage).ToString();

        var args = new { 
            actionSymbol = actionSymbol,
            damageNumber = damageNumber
        };
        AddEntry(
            "action_damage", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddActionScore(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_score", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddActionStop(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_stop", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddActionSubstitution(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };

        AddEntry(
            "action_substitution", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );

        AddEntry(
            "action_substitution", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddElementOffense(Character character, TeamSide teamSide)
    {
        AddEntry(
            "element_offense", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddElementDefense(Character character, TeamSide teamSide)
    {
        AddEntry(
            "element_defense", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDuelStart()
    {
        AddEntry(
            "duel_start", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddDuelWin(Character character, TeamSide teamSide)
    {
        AddEntry(
            "duel_win", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDuelLose(Character character, TeamSide teamSide)
    {
        AddEntry(
            "duel_lose", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            null
        );
    }

    public void AddDuelCancel()
    {
        AddEntry(
            "duel_cancel", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddGoal()
    {
        AddEntry(
            "goal", 
            LogLevel.Trace,
            null,
            default,
            null,
            null
        );
    }

    public void AddPossessionGained(Character character, TeamSide teamSide)
    {
        if (BattleManager.Instance.CurrentPhase == BattlePhase.DeadBall) return;

        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "possession_gained", 
            LogLevel.Trace,
            character,
            teamSide,
            null,
            args
        );
    }

    public void AddCondition(string condition)
    {
        var args = new { 
            condition = condition
        };
        AddEntry(
            "condition", 
            LogLevel.Trace,
            null,
            default,
            null,
            args
        );
    }

    public void AddNotifyMoveLevelUp(Character character, TeamSide teamSide, Move move)
    {
        string characterName = character.CharacterNick;
        string moveName = move.MoveName;

        var args = new {
            characterName = characterName,
            moveName = moveName
        };

        AddEntry(
            "notify_move_evolved", 
            LogLevel.Info,
            character,
            teamSide,
            null,
            null
        );

        /*
        AddEntry(
            "notify_move_level_up", 
            LogLevel.Info,
            character,
            move,
            args
        );
        */
    }

    #endregion

    #region Helper Methods

    #endregion

    #region Other Methods

    public void Clear() => DuelLogEntries.Clear();

    #endregion
}
