using System;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.UI;

public class MenuItemUse : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private SceneGroup sceneMainMenu;
    private MenuStateMachine<MenuItemUseState> stateMachine;
    private Item item;

    #endregion

    #region Override

    private void Start()
    {
        BuildStateMachine();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    #endregion

    #region Button

    public void OnButtonUseClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        switch (item)
        {
            case ItemRecovery itemRecovery:
                stateMachine.Set(MenuItemUseState.WaitingToUseRecovery);
                break;

            case ItemMove itemMove:
                stateMachine.Set(MenuItemUseState.WaitingToUseMove);
                break;
        }
        RequestClose();
    }

    public void OnButtonCancelClicked()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        RequestClose();
    }

    #endregion

    #region Event

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMenuItemUseOpenRequested += HandleOpened;
        UIEvents.OnSelectorCharacterActionClicked += HandleSelectorCharacterActionClicked;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMenuItemUseOpenRequested -= HandleOpened;
        UIEvents.OnSelectorCharacterActionClicked -= HandleSelectorCharacterActionClicked;
    }

    private void HandleOpened(Item item)
    {
        this.item = item;
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleSelectorCharacterActionClicked(Character character)
    {
        if (stateMachine.Is(MenuItemUseState.Idle)) return;
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        ItemManager.Instance.UseItemOnCharacter(item, character);
        stateMachine.Set(MenuItemUseState.Idle);
        UIEvents.RaiseBagUpdated();
    }

    #endregion

    #region StateMachine

    private void BuildStateMachine()
    {
        /*
        stateMachine = new MenuStateMachine<MenuItemUseState>(MenuItemUseState.Idle)
            .OnEnter(MenuItemUseState.Swapping, () =>
            {
                pickedSlot = selectedSlot;
                audioManager.PlaySfxUI("sfx-menu_pick");
                UIEvents.RaiseFormationCharacterSlotUIMoveStarted(selectedSlot);
            })
            .OnExit(MenuItemUseState.Swapping, () =>
            {
                UIEvents.RaiseFormationCharacterSlotUIMoveEnded(pickedSlot);
                pickedSlot = null;
            })
            .OnEnter(MenuItemUseState.Replacing, () =>
            {
                UIEvents.RaiseCharacterSelectorOpenRequested(
                    source:        new SelectorCharacterSourceFromStorage(),
                    action:        new SelectorCharacterAction(),
                    filter:        null,           // or new ExcludeGuidsFilter(currentTeam.GetCharacterGuids(currentBattleType))
                    closeOnSelect: true);
            });
        */

        stateMachine = new MenuStateMachine<MenuItemUseState>(MenuItemUseState.Idle)
            .OnEnter(MenuItemUseState.WaitingToUseRecovery, () =>
            {
                UIEvents.RaiseCharacterSelectorOpenRequested(
                    source:        new SelectorCharacterSourceFromStorageForItemRecovery(item as ItemRecovery),
                    action:        new SelectorCharacterAction(),
                    filter:        null,           // or new ExcludeGuidsFilter(currentTeam.GetCharacterGuids(currentBattleType))
                    closeOnSelect: true);
            })
            .OnEnter(MenuItemUseState.WaitingToUseMove, () =>
            {
                UIEvents.RaiseCharacterSelectorOpenRequested(
                    source:        new SelectorCharacterSourceFromStorage(),
                    action:        new SelectorCharacterAction(),
                    filter:        null,           // or new ExcludeGuidsFilter(currentTeam.GetCharacterGuids(currentBattleType))
                    closeOnSelect: true);
            });
    }

    #endregion

    #region Logic

    #endregion

}
