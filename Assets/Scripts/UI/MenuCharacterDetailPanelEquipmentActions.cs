using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuCharacterDetailPanelEquipmentActions : Menu
{
    [Header("UI References")]
    [SerializeField] private Button buttonEquip;
    [SerializeField] private Button buttonUnequip;

    private EquipmentSlotUI equipmentSlotUI;
    private bool isEquipping = false;

    private void InitializeUI()
    {
        bool isNull = equipmentSlotUI == null || equipmentSlotUI.ItemEquipment == null;

        buttonEquip.gameObject.SetActive(true);
        buttonUnequip.gameObject.SetActive(!isNull);
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonEquipClicked()
    {
        isEquipping = true;
        UIEvents.RaiseEquipmentSelectorOpenRequested(
            new SelectorEquipmentSourceFromStorageByType((EquipmentType)equipmentSlotUI.Index),
            new SelectorEquipmentAction(),
            null
        );
    }

    public void OnButtonUnequipClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        RequestClose();
        UIEvents.RaiseEquipmentActionsCloseRequested(equipmentSlotUI);
        UIEvents.RaiseEquipmentUnequipRequested(equipmentSlotUI.ItemEquipment, equipmentSlotUI.Character);
    }

    public void OnButtonBackClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        RequestClose();
        UIEvents.RaiseEquipmentActionsCloseRequested(equipmentSlotUI);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnEquipmentActionsOpenRequested += HandleOpenRequested;
        UIEvents.OnSelectorEquipmentActionClicked += HandleSelectorEquipmentActionClicked;
        UIEvents.OnEquipmentEquipRequested += HandleEquipmentEquipRequested;
        UIEvents.OnEquipmentUnequipRequested += HandleEquipmentUnequipRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnEquipmentActionsOpenRequested -= HandleOpenRequested;
        UIEvents.OnSelectorEquipmentActionClicked -= HandleSelectorEquipmentActionClicked;
        UIEvents.OnEquipmentEquipRequested -= HandleEquipmentEquipRequested;
        UIEvents.OnEquipmentUnequipRequested -= HandleEquipmentUnequipRequested;
    }

    private void HandleOpenRequested(EquipmentSlotUI equipmentSlotUI)
    {
        this.equipmentSlotUI = equipmentSlotUI;
        InitializeUI();
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleSelectorEquipmentActionClicked(ItemEquipment itemEquipment)
    {
        if (!isEquipping) return;
        UIEvents.RaiseEquipmentEquipRequested(itemEquipment, equipmentSlotUI.Character);
        isEquipping = false;
    }

    private void HandleEquipmentEquipRequested(ItemEquipment itemEquipment, Character character)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        character.EquipEquipment(itemEquipment);
        isEquipping = false;
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseEquipmentActionsCloseRequested(equipmentSlotUI);
    }

    private void HandleEquipmentUnequipRequested(ItemEquipment itemEquipment, Character character)
    {
        character.UnequipEquipment(itemEquipment);
        UIEvents.RaiseCharacterDetailRefreshRequested();
        RequestClose();
        UIEvents.RaiseEquipmentActionsCloseRequested(equipmentSlotUI);
    }

}
