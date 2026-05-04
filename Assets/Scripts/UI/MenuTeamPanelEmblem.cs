using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuTeamPanelEmblem : Menu
{
    [Header("UI References")]
    [SerializeField] private Image imageEmblem;

    private string selectedId;

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonChangeClicked()
    {
        UIEvents.RaiseEmblemSelectorOpened();
    }

    public void OnButtonConfirmClicked()
    {
        if (selectedId != null)
            UIEvents.RaiseTeamEmblemChanged(selectedId);
        RequestClose();
    }

    public void OnButtonCancelClicked()
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamPanelEmblemOpened += HandleTeamPanelEmblemOpened;
        UIEvents.OnTeamEmblemSelected += HandleTeamEmblemSelected;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelEmblemOpened -= HandleTeamPanelEmblemOpened;
        UIEvents.OnTeamEmblemSelected -= HandleTeamEmblemSelected;
    }

    private void HandleTeamPanelEmblemOpened(Sprite emblemSprite)
    {
        imageEmblem.sprite = emblemSprite;
        selectedId = null;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleTeamEmblemSelected(string emblemId, Sprite emblemSprite)
    {
        selectedId = emblemId;
        imageEmblem.sprite = emblemSprite;
    }
}
