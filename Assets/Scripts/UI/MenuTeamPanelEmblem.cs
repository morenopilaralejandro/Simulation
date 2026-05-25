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
    private readonly AddressableBinding<Sprite> _binding = new();
    private Emblem selectedEmblem;

    public override void Hide() 
    {
        _binding.Release();
        _binding.Cancel();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonChangeClicked()
    {
        UIEvents.RaiseTeamEmblemSelectorOpenRequested(
            new SelectorTeamEmblemSource(),
            new SelectorTeamEmblemAction(),
            null
        );
    }

    public void OnButtonConfirmClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        if (selectedEmblem != null)
            UIEvents.RaiseTeamEmblemChanged(selectedEmblem);
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
        UIEvents.OnSelectorTeamEmblemActionClicked += HandleSelectorTeamEmblemActionClicked;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamPanelEmblemOpened -= HandleTeamPanelEmblemOpened;
        UIEvents.OnSelectorTeamEmblemActionClicked -= HandleSelectorTeamEmblemActionClicked;
    }

    private void HandleTeamPanelEmblemOpened(Emblem emblem)
    {
        _ = SetEmblemAsync(emblem);
        selectedEmblem = null;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleSelectorTeamEmblemActionClicked(Emblem emblem)
    {
        selectedEmblem = emblem;
        _ = SetEmblemAsync(emblem);
    }

    private async System.Threading.Tasks.Task SetEmblemAsync(Emblem emblem)
    {
        var task = _binding.LoadAsync(emblem.EmblemAddress);
        imageEmblem.sprite = await task;
    }
}
