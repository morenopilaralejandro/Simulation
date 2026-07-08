using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class SelectorItemStorageSlotBag : Selector<ItemStorageSlot, SelectorItemStorageSlotBagListItem>
{
    #region Fields

    [Header("Field")]
    //[SerializeField] private Variant defaultVariant = Variant.Home;
    [SerializeField] private TMP_Text textGold;
    private SelectorItemStorageSlotSourceFromStorageByCategory src;
    private MenuBagMode mode;
    private int selectedIndex;

    private int currentCategoryIndex = 0;
    private readonly Dictionary<ItemCategory, int> selectedIndices = new();
    private readonly ItemCategory[] categoryOrder =
    {
        ItemCategory.Recovery,
        ItemCategory.Equipment,
        ItemCategory.Move,
        ItemCategory.Material,
        ItemCategory.Misc,
        ItemCategory.Important
    };

    #endregion

    #region Menu Overrides

    protected override void Start()
    {
        src = new SelectorItemStorageSlotSourceFromStorageByCategory(ItemCategory.Recovery);

        foreach (var category in categoryOrder)
            selectedIndices[category] = 0;

        base.Start();
    }

    /*
    public override void Show()
    {
        base.Show();
    }
    */

    /*
    public override void Hide()
    {
        // Reset filter UI when closing.
        UIEvents.RaiseCharacterFilterResetRequested();
        activeFilterData = null;

        base.Hide();
    }
    */

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back,                            HandleBack);
        //im.SubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,         HandleFilterShortcut);
        im.SubscribeDown(CustomAction.Navigation_ShortcutBagCategoryNext, HandleShortcutNext);
        im.SubscribeDown(CustomAction.Navigation_ShortcutBagCategoryPrevious, HandleShortcutPrevious);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back,                          HandleBack);
        //im.UnsubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter,       HandleFilterShortcut);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutBagCategoryNext, HandleShortcutNext);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutBagCategoryPrevious, HandleShortcutPrevious);
    }

    #endregion

    #region Bind

    protected override void Bind(SelectorItemStorageSlotBagListItem view, ItemStorageSlot data)
    {
        view.SetMode(mode);
        view.Bind(data);
    }

    #endregion

    #region Public API

    #endregion

    #region Input

    private void HandleBack()
    {
        RequestClose();
        selectedIndex = GetSelectedIndex();
        if(mode == MenuBagMode.Sell) DialogEvents.RaiseDialogMenuClosed();
    }

    /*

    private void HandleFilterShortcut()
    {
        UIEvents.RaiseCharacterFilterRequested();
    }

    private void HandleSummaryShortcut()
    {
        if (!isDetailShorcutAllow) return;
        var item = GetLastSelectedItem();
        if (item == null || item.Data == null) return;
        UIEvents.RaiseCharacterDetailOpenRequested(item.Data);
    }

    private SelectorCharacterListItem GetLastSelectedItem()
    {
        var view = LastSelected.GetComponent<SelectorCharacterListItem>();
        return view;
    }

    */

    private void HandleShortcutNext()
    {
        currentCategoryIndex++;

        if (currentCategoryIndex >= categoryOrder.Length) currentCategoryIndex = 0;

        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);
    }

    private void HandleShortcutPrevious()
    {
        currentCategoryIndex--;

        if (currentCategoryIndex < 0) currentCategoryIndex = categoryOrder.Length - 1;

        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();
    //public void OnButtonFilterClicked() => HandleFilterShortcut();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnItemStorageSlotBagSelectorOpenRequested += HandleOpenRequested;
        //UIEvents.OnCharacterFilterUpdated         += HandleFilterUpdated;
        UIEvents.OnBagCategoryChanged += HandleBagCategoryChanged;
        ItemEvents.OnCurrencyUpdated += HandleCurrencyUpdated;
        UIEvents.OnBagUpdated += HandleBagUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnItemStorageSlotBagSelectorOpenRequested -= HandleOpenRequested;
        //UIEvents.OnCharacterFilterUpdated         -= HandleFilterUpdated;
        UIEvents.OnBagCategoryChanged -= HandleBagCategoryChanged;
        ItemEvents.OnCurrencyUpdated -= HandleCurrencyUpdated;
        UIEvents.OnBagUpdated -= HandleBagUpdated;
    }

    private void HandleOpenRequested(
        ISelectorSource<ItemStorageSlot>      source,
        ISelectorClickAction<ItemStorageSlot> action,
        ISelectorFilter<ItemStorageSlot>      filter,
        MenuBagMode mode)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
    
        this.mode = mode;
        textGold.text = ItemManager.Instance.GetGold().ToString();

        Open(null, action, filter);

        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);
    }

    /*
    private void HandleFilterUpdated(CharacterFilterData data)
    {
        activeFilterData = data;
        ApplyFilter(new CharacterFilterAdapter(data));
    }
    */

    private void HandleBagCategoryChanged(ItemCategory newCategory)
    {
        // Save the selection of the category we're leaving.
        var oldCategory = categoryOrder[currentCategoryIndex];
        selectedIndices[oldCategory] = Mathf.Max(0, GetSelectedIndex());

        // Switch category.
        currentCategoryIndex = System.Array.IndexOf(categoryOrder, newCategory);
        src.SetCategory(newCategory);
        SetSource(src);

        Refresh();

        // Restore the selection for the category we're entering.
        FocusItem(selectedIndices[newCategory]);
    }

    private void HandleCurrencyUpdated(CurrencyType currencyType, int intValue)
    {
        if(currencyType != CurrencyType.Gold) return;
        textGold.text = ItemManager.Instance.GetGold().ToString();
    }

    private void HandleBagUpdated()
    {
        if (!MenuManager.Instance.IsMenuOpen(this)) return;

        var category = categoryOrder[currentCategoryIndex];

        selectedIndices[category] = Mathf.Max(0, GetSelectedIndex());

        Refresh();
        FocusItem(selectedIndices[category]);
    }

    #endregion
}
