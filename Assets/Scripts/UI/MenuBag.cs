using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuBag : MonoBehaviour
{
    /*
    [Header("Category Tabs")]
    [SerializeField] private List<BagCategoryTab> categoryTabs;
    
    [Header("Item List")]
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemSlotPrefab;
    
    [Header("Item Details Panel")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    
    [Header("Action Buttons")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button giveButton;
    [SerializeField] private Button tossButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject actionPanel;

    [Header("Navigation")]
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    private ItemCategory currentCategory = ItemCategory.Medicine;
    private ItemSlot selectedItem;
    private List<ItemSlotUI> spawnedSlots = new List<ItemSlotUI>();
    private bool isInBattle = false;

    private void OnEnable()
    {
        Inventory.Instance.OnInventoryChanged += RefreshItemList;
        SetupTabButtons();
        SetupNavigation();
        OpenCategory(currentCategory);
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged -= RefreshItemList;
    }

    private void SetupTabButtons()
    {
        foreach (var tab in categoryTabs)
        {
            var category = tab.category;
            tab.GetComponent<Button>().onClick.AddListener(() => OpenCategory(category));
        }
    }

    private void SetupNavigation()
    {
        leftArrow.onClick.AddListener(() => CycleCategory(-1));
        rightArrow.onClick.AddListener(() => CycleCategory(1));
        
        useButton.onClick.AddListener(OnUsePressed);
        cancelButton.onClick.AddListener(CloseBag);
        tossButton.onClick.AddListener(OnTossPressed);
    }

    private void CycleCategory(int direction)
    {
        int categoryCount = System.Enum.GetValues(typeof(ItemCategory)).Length;
        int current = (int)currentCategory;
        current = (current + direction + categoryCount) % categoryCount;
        OpenCategory((ItemCategory)current);
    }

    public void OpenCategory(ItemCategory category)
    {
        currentCategory = category;
        selectedItem = null;
        actionPanel.SetActive(false);

        // Update tab visuals
        foreach (var tab in categoryTabs)
        {
            tab.SetActive(tab.category == category);
        }

        RefreshItemList();
        ClearDetails();
    }

    private void RefreshItemList()
    {
        // Clear existing slots
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        // Populate with current category items
        var items = Inventory.Instance.GetItemsByCategory(currentCategory);

        foreach (var itemSlot in items)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemListParent);
            ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();
            slotUI.Setup(itemSlot, this);
            spawnedSlots.Add(slotUI);
        }
    }

    public void SelectItem(ItemSlot slot)
    {
        selectedItem = slot;

        // Update details panel
        detailIcon.sprite = slot.item.icon;
        detailIcon.enabled = true;
        detailName.text = slot.item.itemName;
        detailDescription.text = slot.item.description;

        // Show action panel
        actionPanel.SetActive(true);

        // Configure available actions
        bool canUse = CanUseItem(slot.item);
        useButton.gameObject.SetActive(canUse);
        tossButton.gameObject.SetActive(!slot.item.isKeyItem);
        giveButton.gameObject.SetActive(!isInBattle);

        // Update selection visuals
        foreach (var uiSlot in spawnedSlots)
        {
            uiSlot.SetSelected(false);
        }
    }

    private bool CanUseItem(ItemData item)
    {
        if (item.useContext == ItemUseContext.Passive) return false;
        if (item.useContext == ItemUseContext.BattleOnly && !isInBattle) return false;
        if (item.useContext == ItemUseContext.OverworldOnly && isInBattle) return false;
        return true;
    }

    private void OnUsePressed()
    {
        if (selectedItem == null) return;

        // Open party selection to choose a Pokémon target
        // This would integrate with your party system
        Debug.Log($"Use {selectedItem.item.itemName} - Open party selection...");
        
        // For now, direct usage example:
        // Inventory.Instance.UseItem(selectedItem.item, targetPokemon);
        
        OnItemUsed?.Invoke(selectedItem.item);
    }

    private void OnTossPressed()
    {
        if (selectedItem == null) return;
        
        // In a full implementation, you'd show a number selector
        Inventory.Instance.RemoveItem(selectedItem.item, 1);
        selectedItem = null;
        actionPanel.SetActive(false);
        ClearDetails();
    }

    private void ClearDetails()
    {
        detailIcon.enabled = false;
        detailName.text = "";
        detailDescription.text = "Select an item.";
    }

    public void OpenBag(bool inBattle = false)
    {
        isInBattle = inBattle;
        gameObject.SetActive(true);
        
        // In battle, default to medicine pocket
        if (inBattle)
            OpenCategory(ItemCategory.Medicine);
    }

    public void CloseBag()
    {
        gameObject.SetActive(false);
        OnBagClosed?.Invoke();
    }

    // Events for external systems
    public System.Action<ItemData> OnItemUsed;
    public System.Action OnBagClosed;
    */
}
