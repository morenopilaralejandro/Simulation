using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Wing;

public class MenuCharacterDetailPanelWingActions : Menu
{
    [Header("UI References")]
    [SerializeField] private Button buttonEquip;
    [SerializeField] private Button buttonUnequip;
    [SerializeField] private Button buttonDetails;

    private WingSlotUI wingSlotUI;
    private bool isEquipping = false;

    private void InitializeUI()
    {
        bool isNull = wingSlotUI == null || wingSlotUI.Wing == null;

        buttonEquip.gameObject.SetActive(true);
        buttonUnequip.gameObject.SetActive(!isNull);
        buttonDetails.gameObject.SetActive(!isNull);
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonEquipClicked()
    {
        isEquipping = true;
        UIEvents.RaiseWingSelectorOpenRequested(
            new SelectorWingSourceFromStorage(),
            new SelectorWingActionEquipt(),
            null,
            true
        );
    }

    public void OnButtonUnequipClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        RequestClose();
        UIEvents.RaiseWingActionsCloseRequested(wingSlotUI);
        UIEvents.RaiseWingUnequipRequested(wingSlotUI.Wing, wingSlotUI.Character);
    }

    public void OnButtonDetailClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaiseWingDetailOpenRequested(wingSlotUI.Wing);
    }

    public void OnButtonBackClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        RequestClose();
        UIEvents.RaiseWingActionsCloseRequested(wingSlotUI);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnWingActionsOpenRequested += HandleOpenRequested;
        UIEvents.OnSelectorWingActionClicked += HandleSelectorWingActionClicked;
        UIEvents.OnWingEquipRequested += HandleWingEquipRequested;
        UIEvents.OnWingUnequipRequested += HandleWingUnequipRequested;
        UIEvents.OnBackFromWingDetailRequested += HandleBackFromWingDetailRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnWingActionsOpenRequested -= HandleOpenRequested;
        UIEvents.OnSelectorWingActionClicked -= HandleSelectorWingActionClicked;
        UIEvents.OnWingEquipRequested -= HandleWingEquipRequested;
        UIEvents.OnWingUnequipRequested -= HandleWingUnequipRequested;
        UIEvents.OnBackFromWingDetailRequested -= HandleBackFromWingDetailRequested;
    }

    private void HandleOpenRequested(WingSlotUI wingSlotUI)
    {
        this.wingSlotUI = wingSlotUI;
        InitializeUI();
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleSelectorWingActionClicked(Wing wing)
    {
        if (!isEquipping) return;
        UIEvents.RaiseWingEquipRequested(wing, wingSlotUI.Character);
        isEquipping = false;
    }

    private void HandleWingEquipRequested(Wing wing, Character character)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");

        if (wing.IsEquipped()) 
            wing.EquippedCharacter.UnequipWing();

        if (character.HasWingEquipped) 
            character.UnequipWing();

        character.EquipWing(wing);
        isEquipping = false;
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseWingActionsCloseRequested(wingSlotUI);
    }

    private void HandleWingUnequipRequested(Wing wing, Character character)
    {
        character.UnequipWing();
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseWingActionsCloseRequested(wingSlotUI);
    }

    private void HandleBackFromWingDetailRequested() 
    {
        RequestClose();
        UIEvents.RaiseWingActionsCloseRequested(wingSlotUI);
    }
}
