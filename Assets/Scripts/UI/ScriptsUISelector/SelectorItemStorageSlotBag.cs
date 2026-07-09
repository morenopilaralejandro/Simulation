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
    [SerializeField] private TMP_Text textGold;
    private SelectorItemStorageSlotSourceFromStorageByCategory src;
    private MenuBagMode mode;

    private int currentCategoryIndex = 0;
    private ItemCategory lastViewedCategory = ItemCategory.Recovery;
    private bool isInitializing = false; // ← ADD THIS FLAG
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

        LogManager.Trace($"[SelectorItemStorageSlotBag] Initialized selectedIndices: {string.Join(", ", selectedIndices)}");

        base.Start();
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back,                            HandleBack);
        im.SubscribeDown(CustomAction.Navigation_ShortcutBagCategoryNext, HandleShortcutNext);
        im.SubscribeDown(CustomAction.Navigation_ShortcutBagCategoryPrevious, HandleShortcutPrevious);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back,                          HandleBack);
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

    #region Input

    private void HandleBack()
    {
        int currentIndex = GetSelectedIndex();
        selectedIndices[src.Category] = Mathf.Max(0, currentIndex);
        lastViewedCategory = src.Category;
        LogManager.Trace($"[HandleBack] Saved index for category {src.Category}: {selectedIndices[src.Category]} (from GetSelectedIndex: {currentIndex})");
        
        RequestClose();
        if(mode == MenuBagMode.Sell) DialogEvents.RaiseDialogMenuClosed();
    }

    private void HandleShortcutNext()
    {
        currentCategoryIndex++;

        if (currentCategoryIndex >= categoryOrder.Length) currentCategoryIndex = 0;

        LogManager.Trace($"[HandleShortcutNext] Moving to category index {currentCategoryIndex}: {categoryOrder[currentCategoryIndex]}");
        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);
    }

    private void HandleShortcutPrevious()
    {
        currentCategoryIndex--;

        if (currentCategoryIndex < 0) currentCategoryIndex = categoryOrder.Length - 1;

        LogManager.Trace($"[HandleShortcutPrevious] Moving to category index {currentCategoryIndex}: {categoryOrder[currentCategoryIndex]}");
        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);
    }

    #endregion

    #region Buttons

    public void OnButtonBackClicked() => HandleBack();

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnItemStorageSlotBagSelectorOpenRequested += HandleOpenRequested;
        UIEvents.OnBagCategoryChanged += HandleBagCategoryChanged;
        ItemEvents.OnCurrencyUpdated += HandleCurrencyUpdated;
        UIEvents.OnBagUpdated += HandleBagUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnItemStorageSlotBagSelectorOpenRequested -= HandleOpenRequested;
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

        // ← SET FLAG BEFORE OPENING
        isInitializing = true;

        currentCategoryIndex = System.Array.IndexOf(categoryOrder, lastViewedCategory);
        LogManager.Trace($"[HandleOpenRequested] Opening bag at category index {currentCategoryIndex}: {categoryOrder[currentCategoryIndex]} (lastViewedCategory: {lastViewedCategory})");

        Open(null, action, filter);

        UIEvents.RaiseBagCategoryChanged(categoryOrder[currentCategoryIndex]);

        // ← CLEAR FLAG AFTER INITIAL CATEGORY CHANGE
        isInitializing = false;
    }

    private void HandleBagCategoryChanged(ItemCategory newCategory)
    {
        if (!isInitializing)
        {
            var currentlyViewedCategory = src.Category;
            int currentSelection = GetSelectedIndex();
            
            if (currentSelection >= 0)
            {
                selectedIndices[currentlyViewedCategory] = currentSelection;
                LogManager.Trace($"[HandleBagCategoryChanged] SAVE: Category {currentlyViewedCategory} -> index {selectedIndices[currentlyViewedCategory]}");
            }
            else
            {
                LogManager.Trace($"[HandleBagCategoryChanged] NO VALID SELECTION to save for {currentlyViewedCategory}");
            }
        }
        else
        {
            LogManager.Trace($"[HandleBagCategoryChanged] SKIPPING SAVE (initializing)");
        }

        currentCategoryIndex = System.Array.IndexOf(categoryOrder, newCategory);
        src.SetCategory(newCategory);
        SetSource(src);

        LogManager.Trace($"[HandleBagCategoryChanged] SWITCH: Category index now {currentCategoryIndex}, new category: {newCategory}");

        int targetIndex = selectedIndices[newCategory];
        LogManager.Trace($"[HandleBagCategoryChanged] RESTORE: About to restore category {newCategory} to index {targetIndex}");

        Refresh();

        LogManager.Trace($"[HandleBagCategoryChanged] After Refresh - GetSelectedIndex: {GetSelectedIndex()}, about to FocusItem({targetIndex})");
        FocusItem(targetIndex);

        LogManager.Trace($"[HandleBagCategoryChanged] After FocusItem - GetSelectedIndex: {GetSelectedIndex()}");
        
        // ← ADD THIS: Check if category is empty
        if (GetSelectedIndex() < 0)
        {
            LogManager.Trace($"[HandleBagCategoryChanged] Category {newCategory} is empty, raising BagDescriptionUpdated with null");
            UIEvents.RaiseBagDescriptionUpdated(null);
        }
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
        int currentSelection = GetSelectedIndex();

        selectedIndices[category] = Mathf.Max(0, currentSelection);

        LogManager.Trace($"[HandleBagUpdated] Category {category} updated. Saved index: {selectedIndices[category]} (from GetSelectedIndex: {currentSelection})");

        Refresh();
        FocusItem(selectedIndices[category]);

        LogManager.Trace($"[HandleBagUpdated] After FocusItem - GetSelectedIndex: {GetSelectedIndex()}");
    }

    #endregion
}
