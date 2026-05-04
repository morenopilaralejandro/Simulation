using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelCharacterActions : Menu
{
    private Character character;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonSummaryClicked() 
    {
        RequestClose();
        UIEvents.RaiseCharacterDetailOpenRequested(character);
    }

    public void OnButtonMoveClicked() 
    {
        RequestClose();
        UIEvents.RaiseFormationCharacterSlotUIMoveRequested(null);
    }

    public void OnButtonReplaceClicked() 
    {
        UIEvents.RaiseFormationCharacterSlotUIReplaceRequested();
    }

    public void OnButtonBackClicked() 
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamCharacterActionsOpenRequested += HandleOpen;
        UIEvents.OnCharacterSelected               += HandleCharacterSelected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamCharacterActionsOpenRequested -= HandleOpen;
        UIEvents.OnCharacterSelected               -= HandleCharacterSelected;
    }

    private void HandleOpen(Character c)
    {
        character = c;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleCharacterSelected(Character _)
    {
        RequestClose();
    }
}
