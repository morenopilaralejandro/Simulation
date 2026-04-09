using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays all existing team loadouts and a "Create New" button.
/// </summary>
public class MenuTeamPanelLoadout : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform listRoot;
    [SerializeField] private LoadoutListItem itemPrefab;
    [SerializeField] private TMP_Text loadoutCountText;
    [SerializeField] private Button createButton;

    private List<LoadoutListItem> spawnedItems = new();
    private TeamManager teamManager;

    private void Awake()
    {
        teamManager = TeamManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #region Show / Hide

    public void Show()
    {
        SetVisible(true);
        Refresh();
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    #endregion

    #region Refresh List

    public void Refresh()
    {
        ClearList();

        List<Team> allLoadouts = teamManager.GetAllLoadouts();
        string activeGuid = teamManager.ActiveLoadoutGuid;

        foreach (Team loadout in allLoadouts)
        {
            LoadoutListItem item = Instantiate(itemPrefab, listRoot);
            bool isActive = loadout.TeamGuid == activeGuid;
            item.Initialize(loadout, isActive, HandleItemClicked);
            spawnedItems.Add(item);
        }

        UpdateCountText(allLoadouts.Count);
        UpdateCreateButtonState(allLoadouts.Count);
    }

    private void ClearList()
    {
        foreach (LoadoutListItem item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }

    private void UpdateCountText(int count)
    {
        if (loadoutCountText != null)
            loadoutCountText.text = $"{count} / {TeamManager.MAX_LOADOUTS}";
    }

    private void UpdateCreateButtonState(int count)
    {
        if (createButton != null)
            createButton.interactable = count < TeamManager.MAX_LOADOUTS;
    }

    #endregion

    #region Handlers

    private void HandleItemClicked(Team team)
    {
        UIEvents.RaiseTeamLoadoutSelected(team);
    }

    private void HandleCreateClicked()
    {
        UIEvents.RaiseTeamLoadoutCreateRequested();
    }

    #endregion
}
