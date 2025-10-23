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
    private DuelParticantsPanel duelParticantsPanel;

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

    public void RegisterDuelPanel(DuelParticantsPanel panel)
    {
        duelParticantsPanel = panel;
    }

    public void UnregisterDuelPanel(DuelParticantsPanel panel)
    {
        duelParticantsPanel = null;
    }
    #endregion

    #region Scoreboard
    public void SetTeam(Team team)
    {
        if (battleScoreboard != null) 
            battleScoreboard.SetTeam(team);
    }

    public void UpdateScore(Team team, int newScore)
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
    public void UpdateTimer(float time)
    {
        if (battleTimer != null) 
            battleTimer.UpdateTimerDisplay(time);
    }

    public void UpdateTimerHalf(TimerHalf timerHalf)
    {
        if (battleTimer != null) 
            battleTimer.UpdateTimerHalfDisplay(timerHalf);
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

    public void ShowDuelPanel()
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.Show();
    }

    public void HideDuelPanel()
    {
        if (duelParticantsPanel != null) 
            duelParticantsPanel.Hide();
    }
    #endregion
}
