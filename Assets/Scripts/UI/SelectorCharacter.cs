using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;

public class SelectorCharacter : Menu
{
    #region Fields

    //[Header("UI References")]
    [SerializeField] private ScrollViewAutoScroll autoScroll;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private RectTransform listItemContainer;
    [SerializeField] private GridLayoutGroup layoutGroup;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;
    private CharacterManager characterManager;
    private TeamManager teamManager;

    private Dictionary<string, Character> auxDict;
    private IReadOnlyDictionary<string, Character> dict;

    private int poolInitialSize = 25;
    private Queue<SelectorCharacterListItem> pool = new Queue<SelectorCharacterListItem>();
    private List<SelectorCharacterListItem> activeItems = new List<SelectorCharacterListItem>();

    private Kit kit;
    private Variant variant;
    private Role role;

    private CharacterFilterData activeFilter;

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
        characterManager = CharacterManager.Instance;
        teamManager = TeamManager.Instance;

        PreWarmPool();
    }

    private void OnDestroy()
    {
        // ReturnAllToPool();
    }

    #endregion

   #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        autoScroll.Activate();
        autoScroll.ResetToTop();

        ReturnAllToPool();
        Populate();
    }

    public override void Hide()
    {
        ReturnAllToPool();
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
        UIEvents.RaiseBackFromCharacterSelectorRequested();
    }

    #endregion

    #region Logic

    private void Populate()
    {
        layoutGroup.enabled = false;

        foreach (var kvp in dict)
        {
            Character character = kvp.Value;

            // Apply filter — skip characters that don't match
            if (activeFilter != null && !activeFilter.IsEmpty)
            {
                if (!activeFilter.Matches(character))
                    continue;
            }

            var listItem = GetFromPool();
            character.ApplyKit(kit, variant, role);
            listItem.Initialize(character);
        }

        layoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(listItemContainer);

        if (activeItems.Count > 0)
            base.SetDefaultSelectable(activeItems[0].Button);
    }

    #endregion

    #region Helper

    private void InitializeFromStorage()
    {
        dict = characterManager.Characters;

        kit = teamManager.ActiveLoadout.Kit;
        variant = Variant.Home;
        role = Role.Field;
    }

    private void InitializeFromTeam(Team team, BattleType battleType)
    {
        auxDict.Clear();
        foreach (string guid in team.GetCharacterGuids(battleType)) 
            auxDict[guid] = characterManager.GetCharacter(guid);
        dict = auxDict;

        kit = team.Kit;
        variant = team.Variant;
        role = Role.Field;
    }

    #endregion

    #region Button Handle

    public void OnButtonBackClicked() 
    {
        Close();
    }

    public void OnButtonFilterClicked() 
    {
        UIEvents.RaiseCharacterFilterRequested();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterSelectorOpened += HandleCharacterSelectorOpened;
        UIEvents.OnCharacterFilterUpdated += HandleCharacterFilterUpdated;
    }

    private void OnDisable()
    {
        UIEvents.OnCharacterSelectorOpened -= HandleCharacterSelectorOpened;
        UIEvents.OnCharacterFilterUpdated -= HandleCharacterFilterUpdated;
    }

    private void HandleCharacterSelectorOpened(Team team, BattleType battleType) 
    {
        activeFilter = null; // reset filter when opening fresh

        if (team == null) 
        {
            InitializeFromStorage();
        } else 
        {
            InitializeFromTeam(team, battleType);
        }

        menuManager.OpenMenu(this);
    }

    private void HandleCharacterFilterUpdated(CharacterFilterData characterFilterData) 
    {
        activeFilter = characterFilterData;

        // Re-populate with the new filter applied
        ReturnAllToPool();
        Populate();
    }

    #endregion

    #region Pool

    private SelectorCharacterListItem GetFromPool()
    {
        SelectorCharacterListItem item;

        if (pool.Count > 0)
        {
            item = pool.Dequeue();
            item.gameObject.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(listItemPrefab, listItemContainer);
            item = go.GetComponent<SelectorCharacterListItem>();
        }

        item.transform.SetAsLastSibling();
        activeItems.Add(item);
        return item;
    }

    private void ReturnToPool(SelectorCharacterListItem item)
    {
        item.Clear();
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    private void ReturnAllToPool()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                ReturnToPool(item);
        }
        activeItems.Clear();
    }

    // Pre-warm the pool on Start
    private void PreWarmPool()
    {
        for (int i = 0; i < poolInitialSize; i++)
        {
            GameObject go = Instantiate(listItemPrefab, listItemContainer);
            var item = go.GetComponent<SelectorCharacterListItem>();
            go.SetActive(false);
            pool.Enqueue(item);
        }
    }

    #endregion

    #region Filter

    #endregion
}
