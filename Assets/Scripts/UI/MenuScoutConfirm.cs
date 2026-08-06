using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Scout;
using Aremoreno.Enums.UI;

public class MenuScoutConfirm : Menu
{
    #region Field

    [Header("UI References")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private TMP_Text textCost;

    private ScoutEntry scoutEntry;

    #endregion

    #region Overrides

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    #endregion

    #region Buttons

    public void OnButtonConfirmClicked()
    {
        //manager scout character
        OnButtonCancelClicked();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMenuScoutConfirmOpenRequested += HandleOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMenuScoutConfirmOpenRequested -= HandleOpened;
    }

    private void HandleOpened(ScoutEntry scoutEntry)
    {
        this.scoutEntry = scoutEntry;
        characterCard.SetCharacter(scoutEntry.Character, scoutEntry.Character.Position);
        textCost.text = scoutEntry.Cost.ToString();
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

    #region Logic

    #endregion

}
