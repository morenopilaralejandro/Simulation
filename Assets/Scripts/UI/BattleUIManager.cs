using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    private BattleScoreboard battleScoreboard;
    private BattleTimer battleTimer;
    private BattleMessage battleMessage;
    private DuelParticantsPanel duelParticantsPanel;
    private DuelMenu duelMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Registration
    public void RegisterScoreboard(BattleScoreboard scoreboard)
    {
        battleScoreboard = scoreboard;
    }

    public void UnregisterScoreboard(BattleScoreboard scoreboard)
    {
        battleScoreboard = null;
    }

    public void RegisterTimer(BattleTimer timer)
    {
        battleTimer = timer;
    }

    public void UnregisterTimer(BattleTimer timer)
    {
        battleTimer = null;
    }

    public void RegisterBattleMessage(BattleMessage battleMessage)
    {
        this.battleMessage = battleMessage;
    }

    public void UnregisterBattleMessage(BattleMessage battleMessage)
    {
        this.battleMessage = null;
    }

    public void RegisterDuelParticipantsPanel(DuelParticantsPanel panel)
    {
        duelParticantsPanel = panel;
    }

    public void UnregisterDuelParticipantsPanel(DuelParticantsPanel panel)
    {
        duelParticantsPanel = null;
    }

    public void RegisterDuelMenu(DuelMenu duelMenu)
    {
        this.duelMenu = duelMenu;
    }

    public void UnregisterDuelMenu(DuelMenu duelMenu)
    {
        this.duelMenu = null;
    }
    #endregion

    #region Scoreboard
    public void SetTeam(Team team)
    {
        if (battleScoreboard != null) 
            battleScoreboard.SetTeam(team);
    }

    public void UpdateScoreDisplay(Team team, int newScore)
    {
        if (battleScoreboard != null) 
        battleScoreboard.UpdateScoreDisplay(team, newScore);
    }

    public void ResetScoreboard()
    {
        if (battleScoreboard != null) 
            battleScoreboard.Reset();
    }
    #endregion

    #region Timer
    public void UpdateTimerDisplay(float time)
    {
        if (battleTimer != null) 
            battleTimer.UpdateTimerDisplay(time);
    }

    public void UpdateTimerHalfDisplay(TimerHalf timerHalf)
    {
        if (battleTimer != null) 
            battleTimer.UpdateTimerHalfDisplay(timerHalf);
    }

    public void HideTimerHalf() 
    {
        if (battleTimer != null) 
            battleTimer.HideTimerHalf();
    }
    #endregion

    #region BattleMessage
    public void SetMessageActive(MessageType messageType, bool isActive)
    {
        if (battleMessage != null) 
            battleMessage.SetMessageActive(messageType, isActive);
    }
    #endregion

    #region Duel Participants
    public void SetDuelParticipant(Character character, List<Character> supports)
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.SetSide(character, supports);
    }

    public void SetDuelCategory(Category category)
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.SetCategory(category);
    }

    public void ShowDuelParticipantsPanel()
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.Show();
    }

    public void HideDuelParticipantsPanel()
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.Hide();
    }
    #endregion

    #region DuelMenu
    public void ShowDuelMenuForUser()
    {
        if (duelMenu != null) 
            duelMenu.Show();
    }

    public void ShowDuelMenuForBoth()
    {
        if (duelMenu != null) 
            duelMenu.Show();
    }

    public void HideDuelMenu()
    {
        if (duelMenu != null) 
            duelMenu.Hide();
    }
    #endregion
}
