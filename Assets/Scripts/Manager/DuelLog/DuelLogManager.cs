using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;
using Simulation.Enums.Log;

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
        //DontDestroyOnLoad(gameObject);
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

    private void HandleBattleStart() 
    { 
        Clear();
        DuelLogManager.Instance.AddMatchStart();
    }
    private void HandleBattleEnd() => AddMatchEnd();
    private void HandleBattlePause(TeamSide teamSide) => AddMatchPause(GoalManager.Instance.Keepers[teamSide]);
    private void HandleBattleResume() => AddMatchResume();

    private void HandlePassPerformed(Character character) => AddActionPass(character);
    private void HandleShootPerformed(Character character, bool isDirect) 
    {
        AddActionShoot(character);
        if (isDirect) AddActionDirect(character);
    }
    private void HandleGoalScored(Character scorringCharacter) => AddActionScore(scorringCharacter);
    private void HandleShootStopped(Character character) => AddActionStop(character);

    private void HandleDuelStart(DuelMode duelMode) => AddDuelStart();
    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser)
    {
        if (isWinnerUser)
            AddDuelWin(winner.Character);
        else
            AddDuelLose(winner.Character);
    }
    private void HandleDuelCancel(DuelMode duelMode) => AddDuelCancel();
    private void HandleGained(Character character) => AddPossessionGained(character);
        



    #endregion

    #region Add Methods

    private void AddEntry(string entryId, LogLevel logLevel, Character character, Move move, object args) 
    {
        DuelLogEntry entry = new DuelLogEntry(entryId, logLevel, character, move, args);
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
            null,
            null
        );
    }

    public void AddMatchPause(Character character)
    {
        var args = new { 
            teamName = character.GetTeam().TeamName
        };
        AddEntry(
            "match_pause", 
            LogLevel.Trace,
            character,
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
            null,
            null
        );
    }

    public void AddDeadballKickoff(Character character)
    {
        AddEntry(
            "deadball_kickoff", 
            LogLevel.Trace,
            character,
            null,
            null
        );
    }

    public void AddActionPass(Character character)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_pass", 
            LogLevel.Trace,
            character,
            null,
            args
        );
    }

    public void AddActionShoot(Character character)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_shoot", 
            LogLevel.Trace,
            character,
            null,
            args
        );
    }

    public void AddActionDirect(Character character)
    {
        AddEntry(
            "action_direct", 
            LogLevel.Trace,
            character,
            null,
            null
        );
    }

    public void AddActionCommand(Character character, DuelCommand command, Move move)
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
            move,
            argsLong
        );

        AddEntry(
            "action_command_short", 
            LogLevel.Info,
            character,
            move,
            argsShort
        );
    }

    public void AddActionDamage(Character character, DuelAction action, float damage)
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
            null,
            args
        );
    }

    public void AddActionScore(Character character)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_score", 
            LogLevel.Trace,
            character,
            null,
            args
        );
    }

    public void AddActionStop(Character character)
    {
        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "action_stop", 
            LogLevel.Trace,
            character,
            null,
            args
        );
    }

    public void AddElementOffense(Character character)
    {
        AddEntry(
            "element_offense", 
            LogLevel.Trace,
            character,
            null,
            null
        );
    }

    public void AddElementDefense(Character character)
    {
        AddEntry(
            "element_defense", 
            LogLevel.Trace,
            character,
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
            null,
            null
        );
    }

    public void AddDuelWin(Character character)
    {
        AddEntry(
            "duel_win", 
            LogLevel.Trace,
            character,
            null,
            null
        );
    }

    public void AddDuelLose(Character character)
    {
        AddEntry(
            "duel_lose", 
            LogLevel.Trace,
            character,
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
            null,
            null
        );
    }

    public void AddPossessionGained(Character character)
    {
        if (BattleManager.Instance.CurrentPhase == BattlePhase.Deadball) return;

        var args = new { 
            characterName = character.CharacterNick
        };
        AddEntry(
            "possession_gained", 
            LogLevel.Trace,
            character,
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
            null,
            args
        );
    }

    #endregion

    #region Helper Methods

    #endregion

    #region Other Methods

    public void Clear() => DuelLogEntries.Clear();

    #endregion
}
