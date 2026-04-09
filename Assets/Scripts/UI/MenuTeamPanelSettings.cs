using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Overlay panel for team settings: rename, set active, delete.
/// </summary>
public class MenuTeamPanelSettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button setActiveButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text activeStatusText;

    private Team currentTeam;
    private TeamManager teamManager;

    private void Awake()
    {
        teamManager = TeamManager.Instance;

        if (closeButton != null)
            closeButton.onClick.AddListener(HandleClose);

        if (setActiveButton != null)
            setActiveButton.onClick.AddListener(HandleSetActive);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(HandleDelete);

        if (nameInputField != null)
            nameInputField.onEndEdit.AddListener(HandleNameChanged);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveAllListeners();

        if (setActiveButton != null)
            setActiveButton.onClick.RemoveAllListeners();

        if (deleteButton != null)
            deleteButton.onClick.RemoveAllListeners();

        if (nameInputField != null)
            nameInputField.onEndEdit.RemoveAllListeners();
    }

    #region Show / Hide

    public void Show(Team team)
    {
        currentTeam = team;
        SetVisible(true);
        PopulateUI();
    }

    public void Hide()
    {
        SetVisible(false);
        currentTeam = null;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    #endregion

    #region Populate

    private void PopulateUI()
    {
        if (currentTeam == null) return;

        if (nameInputField != null)
            nameInputField.text = currentTeam.TeamName ?? "";

        bool isActive = teamManager.ActiveLoadoutGuid == currentTeam.TeamGuid;

        if (activeStatusText != null)
            activeStatusText.text = isActive ? "✦ Active Loadout" : "";

        if (setActiveButton != null)
            setActiveButton.interactable = !isActive;

        // Prevent deleting the last loadout
        if (deleteButton != null)
            deleteButton.interactable = teamManager.Loadouts.Count > 1;
    }

    #endregion

    #region Handlers

    private void HandleNameChanged(string newName)
    {
        if (currentTeam == null) return;
        if (string.IsNullOrWhiteSpace(newName)) return;

        //currentTeam.SetCustoName = newName.Trim();
        TeamEvents.RaiseLoadoutUpdated(currentTeam);
        AudioManager.Instance.PlaySfx("sfx-menu_tap");

        LogManager.Info($"[PanelTeamSettings] Renamed loadout to: {currentTeam.TeamName}");
    }

    private void HandleSetActive()
    {
        if (currentTeam == null) return;

        teamManager.SetActiveLoadout(currentTeam.TeamGuid);
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        PopulateUI(); // Refresh button states
    }

    private void HandleDelete()
    {
        if (currentTeam == null) return;

        // Optional: show confirmation dialog before deleting
        Team teamToDelete = currentTeam;
        bool deleted = teamManager.DeleteLoadout(teamToDelete.TeamGuid);

        if (deleted)
        {
            AudioManager.Instance.PlaySfx("sfx-menu_tap");
            TeamEvents.RaiseLoadoutDeleted(teamToDelete);
        }
    }

    private void HandleClose()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        UIEvents.RaiseTeamSettingsClosed();
    }

    #endregion
}
