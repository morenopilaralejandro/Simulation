using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuWingDetailPanelWingLimitBreak : Menu
{
    [Header("UI References")]
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private MaterialRequirementLayout materialRequirementLayout;

    private Wing wing;
    private LimitBreakRequirementGenerator limitBreakRequirementGenerator = new LimitBreakRequirementGenerator();
    private bool hasRequiredItems;
    private List<MaterialRequirement> requiredItems = new();

    public override void Show()
    {
        Populate();
        base.Show();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    private void Populate() 
    {
        buttonConfirm.interactable = wing.CanLimitBreak() && hasRequiredItems;
        materialRequirementLayout.SetData(requiredItems);
    }

    public void OnButtonConfirmClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        wing.LimitBreak();
        RequestClose();
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnWingLimitBreakPanelOpenRequested += HandleWingLimitBreakPanelOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnWingLimitBreakPanelOpenRequested -= HandleWingLimitBreakPanelOpenRequested;
    }

    private void HandleWingLimitBreakPanelOpenRequested(Wing wing)
    {
        this.wing = wing;

        requiredItems = limitBreakRequirementGenerator.GenerateRequirementWingLimitBreak(wing);
        hasRequiredItems = true;
        foreach (MaterialRequirement materialRequirement in requiredItems) 
        { 
            Item item = ItemFactory.CreateById(materialRequirement.ItemId); 
            int itemCount = ItemManager.Instance.GetItemCount(item); 
            if (itemCount < materialRequirement.Amount) 
            { 
                hasRequiredItems = false; 
                break; 
            } 
        }

        MenuManager.Instance.OpenMenu(this);
    }
}
