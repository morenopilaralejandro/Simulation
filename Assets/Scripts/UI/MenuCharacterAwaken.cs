using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuCharacterAwaken : Menu
{
    [Header("UI References")]
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private MaterialRequirementLayout materialRequirementLayout;

    private Character character;
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
        buttonConfirm.interactable = character.CanAwaken && hasRequiredItems;
        materialRequirementLayout.SetData(requiredItems);
    }

    public void OnButtonConfirmClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        character.Awaken();
        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMenuCharacterAwakenOpenRequested += HandleMenuCharacterAwakenOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMenuCharacterAwakenOpenRequested -= HandleMenuCharacterAwakenOpenRequested;
    }

    private void HandleMenuCharacterAwakenOpenRequested(Character character)
    {
        this.character = character;

        requiredItems = limitBreakRequirementGenerator.GenerateRequirementCharacterAwaken(character);
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
