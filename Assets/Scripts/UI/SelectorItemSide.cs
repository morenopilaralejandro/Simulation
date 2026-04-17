using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Item;

public class SelectorItemSide : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;
    [SerializeField] private ItemCategory category;
    //[SerializeField] private bool showItemCount = false;

    [SerializeField] private ScrollViewAutoScroll autoScroll;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private RectTransform listItemContainer;
    [SerializeField] private Button buttonBack;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private List<ItemStorageSlot> items;
    private List<SelectorItemSideListItem> listItems = new List<SelectorItemSideListItem>();
    
    private BattleType battleType;
    private ItemManager itemManager;

    #endregion

    #region Lifecycle

    private void Awake()
    {

    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
        itemManager = ItemManager.Instance;
    }

    private void OnDestroy()
    {
        ClearList();
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
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    #endregion

    #region Logic

    private void Populate()
    {
        
        ClearList(); // Always clean up first

        items = itemManager.GetItemsByCategory(category);
        foreach (var slot in items)
        {
            if (slot.Item.Category == ItemCategory.Formation && 
                !itemManager.IsFormationOfBattleType(slot.Item, battleType)) continue;

            GameObject go = Instantiate(listItemPrefab, listItemContainer);
            SelectorItemSideListItem listItem = go.GetComponent<SelectorItemSideListItem>();            
            listItem.Initialize(slot);
            listItems.Add(listItem);
        }
        
        if(listItems.Count <= 0) 
            base.SetDefaultSelectable(buttonBack);
        else
            base.SetDefaultSelectable(listItems[0].GetComponent<Button>());
    }

    #endregion

    #region Helper

    private void ClearList()
    {
        items = null;
        foreach (var listItem in listItems)
        {
            if (listItem != null)
                Destroy(listItem.gameObject);
        }
        listItems.Clear();
    }

    #endregion

    #region Button Handle

    public void OnButtonBackClicked() 
    {
        UIEvents.RaiseBackFromSelectorItemSideRequested(category);
        Close();
        ClearList();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnItemSelectorSideOpened += HandleItemSelectorSideOpened;
        UIEvents.OnItemSelected += HandleItemSelected;
    }

    private void OnDisable()
    {
        UIEvents.OnItemSelectorSideOpened -= HandleItemSelectorSideOpened;
        UIEvents.OnItemSelected -= HandleItemSelected;
    }

    private void HandleItemSelectorSideOpened(ItemCategory category, BattleType battleType) 
    {
        this.category = category;
        this.battleType = battleType;
        menuManager.OpenMenu(this);
    }

    private void HandleItemSelected(Item item) 
    {
        Close();
        ClearList();
    }

    #endregion
}
