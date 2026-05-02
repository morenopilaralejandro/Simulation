using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class SelectorMove : Menu
{
    #region Fields

    //[Header("UI References")]
    [SerializeField] private ScrollViewAutoScroll autoScroll;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private RectTransform listItemContainer;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private Button backButton;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private List<Move> moveList;
    private List<SelectorMoveListItem> listItems = new List<SelectorMoveListItem>();

    #endregion

    #region Lifecycle

    private void Awake()
    {
        moveList = new();
    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        autoScroll.Activate();
        autoScroll.ResetToTop();

        Populate();
    }

    public override void Hide()
    {
        autoScroll.Deactivate();

        base.SetInteractable(false);
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);

        if (interactable)
            autoScroll.Activate();
        else
            autoScroll.Deactivate();

        if (interactable) 
            SubscribeInput();
        else
            UnsubscribeInput();
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
        Clear();
    }

    #endregion

    #region Logic

    private void Populate()
    {
        layoutGroup.enabled = false;

        foreach (var move in moveList)
        {
            GameObject go = Instantiate(listItemPrefab, listItemContainer);
            SelectorMoveListItem listItem = go.GetComponent<SelectorMoveListItem>();            
            listItem.Initialize(move);
            listItems.Add(listItem);
        }

        layoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(listItemContainer);

        if (listItems.Count > 0)
            base.SetDefaultSelectable(listItems[0].Button);
        else 
            base.SetDefaultSelectable(backButton);
    }

    private void Clear() 
    {
        moveList.Clear();
        foreach (var listItem in listItems)
        {
            if (listItem != null)
                Destroy(listItem.gameObject);
        }
        listItems.Clear();
    }

    #endregion

    #region Helper
    
    private void InitializeFromTeamExclude(Character character) 
    {
        foreach (var move in character.LearnedMoves) 
        {
            if (!character.IsMoveEquipped(move))
                moveList.Add(move);              
        }
    }

    #endregion

    #region Input 

    private void SubscribeInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    private void UnsubscribeInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    #endregion

    #region Button Handle

    public void OnButtonBackClicked() 
    {
        UIEvents.RaiseBackFromMoveSelectorRequested();
        Close();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnMoveSelectorOpenRequested += HandleMoveSelectorOpenRequested;
        UIEvents.OnMoveSelected += HandleMoveSelected;
    }

    private void OnDisable()
    {
        UIEvents.OnMoveSelectorOpenRequested -= HandleMoveSelectorOpenRequested;
        UIEvents.OnMoveSelected -= HandleMoveSelected;
    }

    private void HandleMoveSelectorOpenRequested(
        MoveSelectorModePopulate modePopulate,
        Character character) 
    {
        Clear();
        switch (modePopulate)
        {
            case MoveSelectorModePopulate.GetFromLearnedExcludeEquiped:
                InitializeFromTeamExclude(character);
                break;
            default:
                InitializeFromTeamExclude(character);
                break;
        }

        menuManager.OpenMenu(this);
    }

    private void HandleMoveSelected(Move move) 
    {
        Close();
    }

    #endregion

}
