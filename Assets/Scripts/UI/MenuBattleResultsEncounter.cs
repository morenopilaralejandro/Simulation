using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class MenuBattleResultsEncounter : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private MenuBattleResultsPanelWinner panelWinner;
    [SerializeField] private MenuBattleResultsPanelGoldXp panelGoldXp;
    [SerializeField] private MenuBattleResultsPanelItemRewards panelItemRewards;
    [SerializeField] private MenuSideCharacterGroupLayout panelParty;

    private BattleResultData data;
    private Coroutine populatePartyCoroutine;
    private float coroutineDuration = 0.3f;

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
        CancelPopulatePartyCoroutine();
        BattleEvents.RaiseResultsContinueRequested();
    }

    #endregion

    #region Logic

    private void Populate() 
    {
        panelWinner.SetData(data);

        panelGoldXp.SetData(data.GoldReward, data.XpReward);

        panelItemRewards.SetData(data.ItemRewards);

        CancelPopulatePartyCoroutine();
        panelParty.Populate();
        panelParty.AnimateXp(data.XpResult);
    }

    private void Clear() 
    {
        CancelPopulatePartyCoroutine();
    }

    #endregion

    #region Coroutine

    private IEnumerator PopulatePartyDelayed()
    {
        yield return new WaitForSeconds(coroutineDuration);

        panelParty.Populate();
        populatePartyCoroutine = null;
    }

    private void CancelPopulatePartyCoroutine()
    {
        if (populatePartyCoroutine != null)
        {
            StopCoroutine(populatePartyCoroutine);
            populatePartyCoroutine = null;
        }
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnResultsEncounterOpenRequested += HandleResultsEncounterOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnResultsEncounterOpenRequested -= HandleResultsEncounterOpenRequested;
    }

    private void HandleResultsEncounterOpenRequested(BattleResultData data)
    {
        this.data = data;
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

}
