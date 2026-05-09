using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

public class MenuSide : Menu
{
    [Header("UI References - Sub menues")]
    [SerializeField] private MenuTeam menuTeam;
    [SerializeField] private MenuSave menuSave;
    [SerializeField] private MenuQuitOverworld menuQuit;
    [SerializeField] private MenuCharacter menuCharacter;

    [Header("UI References - Internal")]
    [SerializeField] protected ScrollViewAutoScroll autoScroll;
    //[SerializeField] protected ScrollRect           scrollRect;
    [SerializeField] protected MenuSideLayout layout;

    private MenuManager menuManager;
    private WorldManager worldManager;

    private int openedFrame;

    private void Start() 
    {
        menuManager = MenuManager.Instance;
        worldManager = WorldManager.Instance;
    }

    //show hide populate and clear layout

    public override void Show() 
    {
        //layout.Populate();
        base.Show();
    }

    public override void Hide() 
    {
        base.Hide();
        layout.Clear();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);
        if (autoScroll != null)
        {
            if (interactable) autoScroll.Activate();
            else              autoScroll.Deactivate();
        }

        if (interactable) layout.Populate();
    }

    protected override void OnGainedInput()
    {
        var input = InputManager.Instance;
        input.UnsubscribeDown(CustomAction.World_OpenSideMenu, HandleOpenInput);
        input.SubscribeDown(CustomAction.World_CloseSideMenu, OnButtonBackClicked);
    }

    protected override void OnLostInput()
    {
        var input = InputManager.Instance;
        input.UnsubscribeDown(CustomAction.World_CloseSideMenu, OnButtonBackClicked);
        input.SubscribeDown(CustomAction.World_OpenSideMenu, HandleOpenInput);
    }

    public void OnButtonTeamTapped()
    {
        menuManager.OpenMenu(menuTeam);
    }

    public void OnButtonSaveTapped()
    {
        menuManager.OpenMenu(menuSave);
    }

    public void OnButtonCharacterTapped()
    {
        menuManager.OpenMenu(menuCharacter);
    }

    public void OnButtonQuitTapped()
    {
        menuManager.OpenMenu(menuQuit);
    }

    public void OnButtonBackClicked()
    {
        if (Time.frameCount == openedFrame) return;

        if (!IsInteractable()) return;
        EventSystem.current.SetSelectedGameObject(null);
        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.FreeRoam);
        UIEvents.RaiseMenuSideCloseRequested();
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMenuSideOpenRequested += HandleMenuSideOpenRequested;

        // Start in "world" mode, so only open is active.
        InputManager.Instance.SubscribeDown(CustomAction.World_OpenSideMenu, HandleOpenInput);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMenuSideOpenRequested -= HandleMenuSideOpenRequested;

        var input = InputManager.Instance;
        input.UnsubscribeDown(CustomAction.World_OpenSideMenu, HandleOpenInput);
        input.UnsubscribeDown(CustomAction.World_CloseSideMenu, OnButtonBackClicked);
    }

    private void HandleMenuSideOpenRequested()
    {
        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.InMenu);
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleOpenInput()
    {
        if (!WorldManager.Instance.PlayerWorldEntity.CanOpenMenu) return;

        openedFrame = Time.frameCount;

        worldManager.PlayerWorldEntity.SetState(PlayerWorldState.InMenu);
        MenuManager.Instance.OpenMenu(this);
    }
}
