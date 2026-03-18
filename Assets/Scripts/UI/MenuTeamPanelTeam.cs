using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;

/// <summary>
/// Displays the selected team's formation and characters for a given BattleType.
/// </summary>
public class MenuTeamPanelTeam : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text teamNameText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button toggleBattleTypeButton;
    [SerializeField] private TMP_Text battleTypeLabel;
    [SerializeField] private FormationLayoutUI formationLayoutUI;

    private Team currentTeam;
    private BattleType currentBattleType;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() => UIEvents.RaiseBackFromTeamRequested());

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => UIEvents.RaiseTeamSettingsRequested());

        if (toggleBattleTypeButton != null)
            toggleBattleTypeButton.onClick.AddListener(HandleToggleBattleType);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveAllListeners();

        if (settingsButton != null)
            settingsButton.onClick.RemoveAllListeners();

        if (toggleBattleTypeButton != null)
            toggleBattleTypeButton.onClick.RemoveAllListeners();
    }

    #region Show / Hide

    public void Show(Team team, BattleType battleType)
    {
        currentTeam = team;
        currentBattleType = battleType;

        SetVisible(true);
        PopulateUI();
    }

    public void Refresh(Team team, BattleType battleType)
    {
        currentTeam = team;
        currentBattleType = battleType;
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

        if (teamNameText != null)
            teamNameText.text = currentTeam.TeamName ?? "Unnamed Team";

        if (battleTypeLabel != null)
            battleTypeLabel.text = currentBattleType == BattleType.Full ? "Full Battle" : "Mini Battle";

        // Initialize the formation layout with current team and battle type
        if (formationLayoutUI != null)
            formationLayoutUI.Initialize(currentTeam, currentBattleType);
    }

    #endregion

    #region Battle Type Toggle

    private void HandleToggleBattleType()
    {
        BattleType newType = currentBattleType == BattleType.Full
            ? BattleType.Mini
            : BattleType.Full;

        UIEvents.RaiseBattleTypeChanged(newType, currentBattleType);
    }

    #endregion
}
