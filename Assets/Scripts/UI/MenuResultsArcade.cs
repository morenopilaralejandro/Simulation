using System;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class MenuResultsArcade : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private Image imageMatchRankBest;
    [SerializeField] private MenuResultsPanelWinner panelWinner;
    [SerializeField] private BattleScoreboard panelScoreboard;

    private BattleResultData data;

    #endregion

    #region Override

    public override void Show()
    {
        Populate();
        base.Show();
        /*
        if (autoScroll != null)
        {
            autoScroll.Activate();
            autoScroll.ResetToTop();
        }
        */
    }

    public override void Hide()
    {
        Clear();
        //ReturnAllToPool();
        //if (autoScroll != null) autoScroll.Deactivate();
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);
        /*
        if (autoScroll != null)
        {
            if (interactable) autoScroll.Activate();
            else              autoScroll.Deactivate();
        }
    */
    }

    #endregion

    #region Input    

    /*
    protected override void OnGainedInput() 
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    protected override void OnLostInput() 
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }
    */
    
    #endregion

    #region Buttons

    /*
    public void OnButtonBackClicked()
    {
        RequestClose();
    }
    */

    public async void OnButtonContinueClicked()
    {
        BattleEvents.RaiseResultsContinueRequested();
    }

    #endregion

    #region Logic

    private void Populate() 
    {
        panelWinner.SetData(data);
        panelScoreboard.SetTeamAsync(data.Teams[TeamSide.Home], TeamSide.Home);
        panelScoreboard.SetTeamAsync(data.Teams[TeamSide.Away], TeamSide.Away);

        panelScoreboard.UpdateScoreDisplay(TeamSide.Home, data.Scores[TeamSide.Home]);
        panelScoreboard.UpdateScoreDisplay(TeamSide.Away, data.Scores[TeamSide.Away]);

        imageMatchRankBest.sprite = IconManager.Instance.MatchRank.GetIcon(data.MatchRank);
    }

    private void Clear() 
    {

    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnResultsArcadeOpenRequested += HandleResultsArcadeOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnResultsArcadeOpenRequested -= HandleResultsArcadeOpenRequested;
    }

    private void HandleResultsArcadeOpenRequested(BattleResultData data)
    {
        this.data = data;
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

}
