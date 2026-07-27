using System;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class MenuBattleResultsMatchNode : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private Image imageMatchRankBest;
    [SerializeField] private MenuBattleResultsPanelWinner panelWinner;
    [SerializeField] private MenuBattleResultsPanelGoldXp panelGoldXp;
    [SerializeField] private BattleScoreboard panelScoreboard;
    [SerializeField] private MenuBattleResultsPanelItemRewards panelItemRewards;

    private BattleResultData data;

    #endregion

    #region Override

    public override void Show()
    {
        if(data.IsUserWin) 
            AudioManager.Instance.PlayBgm("bgm-fanfare");
        else
            AudioManager.Instance.PlayBgm("bgm-gameover");

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
        AudioManager.Instance.StopBgm();
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

    public void OnButtonContinueClicked()
    {
        BattleEvents.RaiseResultsContinueRequested();
    }

    #endregion

    #region Logic

    private void Populate() 
    {
        panelWinner.SetData(data);

        _ = panelScoreboard.SetTeamAsync(data.Teams[TeamSide.Home], TeamSide.Home);
        _ = panelScoreboard.SetTeamAsync(data.Teams[TeamSide.Away], TeamSide.Away);

        panelScoreboard.UpdateScoreDisplay(TeamSide.Home, data.Scores[TeamSide.Home]);
        panelScoreboard.UpdateScoreDisplay(TeamSide.Away, data.Scores[TeamSide.Away]);

        panelGoldXp.SetData(data.GoldReward, data.XpReward);

        panelItemRewards.SetData(data.ItemRewards);

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
        UIEvents.OnResultsMatchNodeOpenRequested += HandleResultsMatchNodeOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnResultsMatchNodeOpenRequested -= HandleResultsMatchNodeOpenRequested;
    }

    private void HandleResultsMatchNodeOpenRequested(BattleResultData data)
    {
        this.data = data;
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

}
