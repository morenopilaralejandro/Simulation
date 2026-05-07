using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Team;
using Aremoreno.Enums.Input;

public class TeamPreviewUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FormationLayoutUI formationLayoutUI;

    private TeamSide localSide;

    // Cache to avoid redundant GetUserSide() calls
    private bool localSideCached;

    private void Start()
    {
        HideCanvasGroup();
    }

    #region Events

    private void OnEnable()
    {
        UIEvents.OnFormationCharacterSlotUISelectedDefault += HandleFormationCharacterSlotUISelectedDefault;
        TeamEvents.OnTeamPreviewStateChanged += HandleStateChanged;
        TeamEvents.OnTeamPreviewPageChanged += HandlePageChanged;
        TeamEvents.OnTeamPreviewStarted+= HandleTeamPreviewStarted;
        TeamEvents.OnTeamPreviewEnded += HandleTeamPreviewEnded;
        UIEvents.OnFormationCharacterSlotUIHighlighted += HandleFormationCharacterSlotUIHighlighted;

        //  REMOVED: Subscriptions to events with empty handlers
        // OnTeamPreviewSideStateChanged, OnTeamPreviewSideReady, OnTeamPreviewReady
        // were doing nothing — each subscription is a delegate invocation cost
    }

    private void OnDisable()
    {
        UIEvents.OnFormationCharacterSlotUISelectedDefault -= HandleFormationCharacterSlotUISelectedDefault;
        TeamEvents.OnTeamPreviewStateChanged -= HandleStateChanged;
        TeamEvents.OnTeamPreviewPageChanged -= HandlePageChanged;
        TeamEvents.OnTeamPreviewStarted -= HandleTeamPreviewStarted;
        TeamEvents.OnTeamPreviewEnded -= HandleTeamPreviewEnded;
        UIEvents.OnFormationCharacterSlotUIHighlighted -= HandleFormationCharacterSlotUIHighlighted;
    }

    #endregion

    #region Handlers

    private void HandleStateChanged(TeamPreviewState state)
    {
        //  Cache localSide once, not every state change
        if (!localSideCached)
        {
            localSide = BattleManager.Instance.GetUserSide();
            localSideCached = true;
        }

        bool isVisible = state != TeamPreviewState.Inactive
                      && state != TeamPreviewState.Finished;

        if (isVisible)
            ShowCanvasGroup();
        else
            HideCanvasGroup();
    }

    private void HandlePageChanged(TeamSide side, Team team, BattleType battleType, int page, int totalPages)
    {
        if (side != localSide) return;
        formationLayoutUI.Initialize(team, battleType, MenuTeamMode.Battle);
    }

    private void HandleTeamPreviewStarted()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, UIEvents.RaiseCharacterDetailSideNextPageRequested);

    }

    private void HandleTeamPreviewEnded()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_ShortcutTeamCharacterNext, UIEvents.RaiseCharacterDetailSideNextPageRequested);

        HideCanvasGroup();
        localSideCached = false;
        formationLayoutUI.ReleaseAll();
    }

    private void HandleFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot) 
    {
        if (!canvasGroup.interactable) return;
        if (slot == null || slot.gameObject == null) return;
        if (InputManager.Instance.IsAndroid && !InputManager.Instance.IsUsingController) return;


        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);
        EventSystem.current.SetSelectedGameObject(slot.gameObject);
        UIEvents.RaiseFormationCharacterSlotUISelected(slot);
    }

    private void HandleFormationCharacterSlotUIHighlighted(FormationCharacterSlotUI slot) 
    {
        if (!canvasGroup.interactable) return;
        if (slot == null || slot.gameObject == null) return;
        EventSystem.current.SetSelectedGameObject(slot.gameObject);
    }

    #endregion

    #region Canvas Group Helpers

    private void ShowCanvasGroup()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideCanvasGroup()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    #endregion

    #region Buttons

    public void OnTeamPreviewButtonContinueClicked() 
    {
        UIEvents.RaiseTeamPreviewButtonContinueClicked();
    }

    #endregion
}
