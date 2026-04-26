using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Team;

public class TeamPreviewManager : MonoBehaviour
{
    #region Fields

    public static TeamPreviewManager Instance;

    private BattleManager battleManager;
    private InputManager inputManager;
    private bool isMultiplayer;
    private bool isAutoBattleEnabled;
    private bool isPollingInput; // Simple flag instead of toggling enabled
    private TeamPreviewState state = TeamPreviewState.Inactive;
    private readonly Dictionary<TeamSide, TeamPreviewSideData> sides = new();

    public TeamPreviewState State => state;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        battleManager = BattleManager.Instance;
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        // Single bool check — nearly zero cost when inactive
        if (!isPollingInput) return;

        if (state == TeamPreviewState.WaitingForReady && AreBothReady())
        {
            StartBattle();
            return;
        }

        HandleConfirmInput();
    }

    #endregion

    #region Interface

    public void StartTeamPreview(Team homeTeam, Team awayTeam)
    {
        if(inputManager.IsAndroid) InputEvents.RaiseScreenControlsHideRequested();
        TeamEvents.RaiseTeamPreviewStarted();

        isMultiplayer = false;
        isAutoBattleEnabled = AutoBattleManager.Instance.IsAutoBattleEnabled;
        var battleType = battleManager.CurrentType;
        sides[TeamSide.Home] = new TeamPreviewSideData(TeamSide.Home, homeTeam, awayTeam, battleType);
        sides[TeamSide.Away] = new TeamPreviewSideData(TeamSide.Away, awayTeam, homeTeam, battleType);

        battleManager.Freeze();
        battleManager.SetBattlePhase(BattlePhase.TeamPreview);

        SetState(TeamPreviewState.Previewing);

        sides[TeamSide.Home].ShowFirstPage();
        sides[TeamSide.Away].ShowFirstPage();

        if (isAutoBattleEnabled)
        {
            if (isMultiplayer)
            {
                TeamSide userSide = battleManager.GetUserSide();
                ConfirmSide(userSide);
                ConfirmSide(userSide);
            }
            else
            {
                StartBattle();
                return;
            }
        }

        isPollingInput = true; //  Start polling
    }

    public TeamPreviewSideData GetSideData(TeamSide side) => sides.GetValueOrDefault(side);

    #endregion

    #region Input

    private void HandleConfirmInput()
    {
        if (!inputManager.GetDown(CustomAction.BattleUI_TeamPreviewConfirm))
            return;

        if (isMultiplayer)
        {
            TeamSide userSide = battleManager.GetUserSide();
            ConfirmSide(userSide);
        }
        else
        {
            ConfirmSide(TeamSide.Home);
            ConfirmSide(TeamSide.Away);
        }
    }

    public void ConfirmSide(TeamSide side)
    {
        if (state != TeamPreviewState.Previewing && state != TeamPreviewState.WaitingForReady)
            return;

        if (!sides.TryGetValue(side, out var sideData)) return;

        sideData.Confirm();

        if (sideData.IsReady)
            CheckAllReady();
    }

    #endregion

    #region Flow

    private void CheckAllReady()
    {
        if (!AreBothReady())
        {
            SetState(TeamPreviewState.WaitingForReady);
            return;
        }

        TeamEvents.RaiseTeamPreviewReady();
        SetState(TeamPreviewState.WaitingForReady);
    }

    private void StartBattle()
    {
        isPollingInput = false; //  Stop polling — Update() becomes a single bool check

        SetState(TeamPreviewState.Finished);

        sides.Clear(); //  Release preview data for GC

        TeamEvents.RaiseTeamPreviewEnded();
        if(inputManager.IsAndroid) InputEvents.RaiseScreenControlsShowRequested();
    }

    #endregion

    #region Helpers

    public bool IsActive => state != TeamPreviewState.Inactive
                         && state != TeamPreviewState.Finished;

    public bool IsSideReady(TeamSide side) =>
        sides.TryGetValue(side, out var data) && data.IsReady;

    public bool AreBothReady() =>
        sides.TryGetValue(TeamSide.Home, out var home) && home.IsReady
        && sides.TryGetValue(TeamSide.Away, out var away) && away.IsReady;

    private void SetState(TeamPreviewState newState)
    {
        state = newState;
        TeamEvents.RaiseTeamPreviewStateChanged(newState);
    }

    #endregion

    #region Events

    //  Events stay subscribed regardless of polling state
    private void OnEnable()
    {
        TeamEvents.OnTeamPreviewRequested += HandleTeamPreviewRequested;
        UIEvents.OnTeamPreviewButtonContinueClicked += HandleTeamPreviewButtonContinueClicked;
    }

    private void OnDisable()
    {
        TeamEvents.OnTeamPreviewRequested -= HandleTeamPreviewRequested;
        UIEvents.OnTeamPreviewButtonContinueClicked -= HandleTeamPreviewButtonContinueClicked;
    }

    private void HandleTeamPreviewRequested()
    {
        StartTeamPreview(
            battleManager.Teams[TeamSide.Home],
            battleManager.Teams[TeamSide.Away]
        );
    }

    private void HandleTeamPreviewButtonContinueClicked(TeamSide teamSide)
    {
        ConfirmSide(teamSide);
    }

    #endregion
}
