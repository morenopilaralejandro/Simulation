using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;
using Simulation.Enums.UI;

public class SelectorTeamEmblem : Menu
{
    #region Fields

    //[Header("UI References")]
    //[SerializeField] private MenuTeamMode mode;
    [SerializeField] private ScrollViewAutoScroll autoScroll;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private RectTransform listItemContainer;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;

    private Dictionary<string, Sprite> dict;
    private List<SelectorTeamEmblemListItem> listItems = new List<SelectorTeamEmblemListItem>();

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

        dict = SpriteAtlasManager.Instance.GetAllSpritesFromAtlasTeamCrest();
        foreach (var kvp in dict)
        {
            GameObject go = Instantiate(listItemPrefab, listItemContainer);
            SelectorTeamEmblemListItem listItem = go.GetComponent<SelectorTeamEmblemListItem>();            
            listItem.Initialize(kvp.Key, kvp.Value);
            listItems.Add(listItem);
        }
        base.SetDefaultSelectable(listItems[0].GetComponent<Button>());
    }

    #endregion

    #region Helper

    private void ClearList()
    {
        if (dict != null)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value);
            }
            dict.Clear();
            dict = null;
        }

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
        Close();
        ClearList();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnEmblemSelectorOpened += HandleEmblemSelectorOpened;
        UIEvents.OnTeamEmblemSelected += HandleTeamEmblemSelected;
        UIEvents.OnTeamEmblemChanged += HandleTeamEmblemChanged;
    }

    private void OnDisable()
    {
        UIEvents.OnEmblemSelectorOpened -= HandleEmblemSelectorOpened;
        UIEvents.OnTeamEmblemSelected -= HandleTeamEmblemSelected;
        UIEvents.OnTeamEmblemChanged -= HandleTeamEmblemChanged;
    }

    private void HandleEmblemSelectorOpened() 
    {
        menuManager.OpenMenu(this);
    }

    private void HandleTeamEmblemSelected(string emblemId, Sprite emblemSprite) 
    {
        Close();
    }

    public void HandleTeamEmblemChanged(string emblemId)
    {
        ClearList();
    }

    #endregion
}
