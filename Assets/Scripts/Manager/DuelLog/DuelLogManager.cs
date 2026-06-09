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

    // TODO refactor to use DuelLogData instead of passing all the arguments

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

    private void AddEntry(DuelLogEntryData data)
    {
        var entry = new DuelLogEntry(data);
        DuelLogEntries.Add(entry);
        LogManager.Trace($"[DuelLogManager] Shown: {data.LogLevel == LogLevel.Info} String: {entry.EntryString}");
        DuelLogEvents.RaiseNewEntry(entry);
    }

    public void AddMatchStart() => AddEntry(Trace("match_start"));

    public void AddMatchHalf() => AddEntry(Trace("match_half"));

    public void AddMatchEnd() => AddEntry(Trace("match_end"));

    public void AddMatchPause(CharacterEntityBattle character)
    {
        var args = new { 
            teamName = character.GetTeam().TeamName
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "match_pause",
            LogLevel = LogLevel.Trace,
            Character = character.Character,
            TeamSide = character.TeamSide,
            Args = args
        });
    }

    public void AddMatchResume() => AddEntry(Trace("match_resume"));

    public void AddDeadBallKickoff(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_kickoff",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDeadBallThrowIn(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_throw_in",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDeadBallCornerKick(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_corner_kick",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDeadBallGoalKick(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_goal_kick",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDeadBallFreeKickDirect(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_free_kick_direct",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDeadBallFreeKickIndirect(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "deadball_free_kick_indirect",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddActionPass(Character character, TeamSide teamSide)
    {
        if (DeadBallManager.Instance.IsFirstKickoff) return;

        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_pass",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionShoot(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_shoot",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionDirect(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_direct",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide
        });
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

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_command",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Move = move,
            Args = argsLong
        });
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
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_damage",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionScore(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_score",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionStop(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_stop",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionSubstitution(Character character, TeamSide teamSide)
    {
        var args = new { 
            characterName = character.CharacterNick
        };

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_substitution",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_substitution",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddActionWingActivate(Character character, TeamSide teamSide, Wing wing)
    {
        var args = new { 
            wingName = wing.WingName
        };

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_wing_activate",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Wing = wing,
            Args = args
        });

        /*
        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_wing_activate_short",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Wing = wing,
            Args = argsLong
        });
        */
    }

    public void AddActionWingDeactivate(Character character, TeamSide teamSide, Wing wing)
    {
        /*
        var args = new { 
            wingName = wing.WingName
        };

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_wing_activate",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Wing = wing,
            Args = args
        });
        */

        AddEntry(new DuelLogEntryData
        {
            EntryId = "action_wing_deactivate",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Wing = wing
        });
    }

    public void AddElementOffense(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "element_offense",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddElementDefense(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "element_defense",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDuelStart() => AddEntry(Trace("duel_start"));

    public void AddDuelWin(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "duel_win",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDuelLose(Character character, TeamSide teamSide)
    {
        AddEntry(new DuelLogEntryData
        {
            EntryId = "duel_lose",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide
        });
    }

    public void AddDuelCancel() => AddEntry(Trace("duel_cancel"));

    public void AddGoal() => AddEntry(Trace("goal"));

    public void AddPossessionGained(Character character, TeamSide teamSide)
    {
        if (BattleManager.Instance.CurrentPhase == BattlePhase.DeadBall) return;

        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "possession_gained",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Args = args
        });
    }

    public void AddCondition(string condition)
    {
        var args = new { 
            condition = condition
        };
        AddEntry(new DuelLogEntryData
        {
            EntryId = "condition",
            LogLevel = LogLevel.Trace,
            Args = args
        });
    }

    public void AddNotifyMoveEvolved(Character character, TeamSide teamSide, Move move)
    {   
        /*
        string characterName = character.CharacterNick;
        string moveName = move.MoveName;

        var args = new {
            characterName = characterName,
            moveName = moveName
        };

        AddEntry(new DuelLogEntryData
        {
            EntryId = "notify_move_evolved",
            LogLevel = LogLevel.Trace,
            Character = character,
            TeamSide = teamSide,
            Move move = move,
            Args = args
        });

        AddEntry(new DuelLogEntryData
        {
            EntryId = "notify_move_evolved",
            LogLevel = LogLevel.Info,
            Character = character,
            TeamSide = teamSide,
            Move move = move,
            Args = args
        });
        */

        AddEntry(Trace("notify_move_evolved"));
        AddEntry(Info("notify_move_evolved"));

    }

    public void AddNotifyWingEvolved(Character character, TeamSide teamSide, Wing wing)
    {
        AddEntry(Trace("notify_wing_evolved"));
        AddEntry(Info("notify_wing_evolved"));
    }

    #endregion

    #region Helper Methods

    private DuelLogEntryData Trace(string entryId) => new() { EntryId = entryId, LogLevel = LogLevel.Trace };
    private DuelLogEntryData Info(string entryId) => new() { EntryId = entryId, LogLevel = LogLevel.Info };

    #endregion

    #region Other Methods

    public void Clear() => DuelLogEntries.Clear();

    #endregion
}
