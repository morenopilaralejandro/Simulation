using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelName : Menu
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputFieldName;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonConfirmClicked()
    {
        UIEvents.RaiseTeamNameChanged(inputFieldName.text);
        RequestClose();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
    }

    public void OnInputValueChanged(string currentText)
    {

    }

    public void OnInputEndEdit()
    {
        OnButtonConfirmClicked();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelNameOpened += HandleTeamPanelNameOpened;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelNameOpened -= HandleTeamPanelNameOpened;
    }

    private void HandleTeamPanelNameOpened(string teamName)
    {
        inputFieldName.text = teamName;
        MenuManager.Instance.OpenMenu(this);
    }
}
