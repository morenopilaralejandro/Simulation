using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.World;

public class MenuMatchDetail : Menu
{
    [Header("UI")]
    [SerializeField] private Selectable defaultSelectableAfterSlot;
    [SerializeField] private FormationLayoutUI formationLayoutUI;

    [Header("Match Rank Rewards")]
    [SerializeField] private CanvasGroup panelMatchInfo;
    [SerializeField] private MatchRankRewardIU rewardBronze;
    [SerializeField] private MatchRankRewardIU rewardSilver;
    [SerializeField] private MatchRankRewardIU rewardGold;
    [SerializeField] private Image imageMatchRankBest;

    [Header("Options")]
    [SerializeField] private Toggle toggleAutoBattle;

    private Match currentMatch;
    private MatchChainNodeMatch currentNode;

    #region Unity

    public override void SetInteractable(bool isInteractable)
    {
        base.SetInteractable(isInteractable);
        PopulateUI();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        UIEvents.OnMatchDetailOpened += HandleMatchDetailOpened;
        UIEvents.OnFormationCharacterSlotUISelectedDefault += HandleFormationCharacterSlotUISelectedDefault;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UIEvents.OnMatchDetailOpened -= HandleMatchDetailOpened;
        UIEvents.OnFormationCharacterSlotUISelectedDefault -= HandleFormationCharacterSlotUISelectedDefault;
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    private void PopulateUI()
    {
        var team = DatabaseManager.Instance.GetTeam(currentMatch.TeamId);
        StorySystemManager.Instance.PopulateTeamWithCharacters(team, currentMatch.BattleType, currentMatch.Level);
        formationLayoutUI.Initialize(team, currentMatch.BattleType, MenuTeamMode.Battle);

        toggleAutoBattle.SetIsOnWithoutNotify(AutoBattleManager.Instance.IsAutoBattleEnabled);

        if (currentNode == null)
        {
            panelMatchInfo.alpha = 0f;
            panelMatchInfo.interactable = false;
            panelMatchInfo.blocksRaycasts = false;
        }
        else
        {
            panelMatchInfo.alpha = 1f;
            panelMatchInfo.interactable = true;
            panelMatchInfo.blocksRaycasts = true;

            rewardBronze.Setup(MatchRank.B, currentNode.DropIdB);
            rewardSilver.Setup(MatchRank.A, currentNode.DropIdA);
            rewardGold.Setup(MatchRank.S, currentNode.DropIdS);

            imageMatchRankBest.sprite = IconManager.Instance.MatchRank.GetIcon(currentNode.MatchRank);
        }
    }

    #endregion

    #region Events

    private void HandleMatchDetailOpened(string matchId, MatchChainNodeMatch node = null)
    {
        currentMatch = MatchFactory.CreateById(matchId);
        currentNode = node;

        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot) 
    {
        if (!IsInteractable()) return;
        if (slot == null || slot.gameObject == null) return;

        UIEvents.RaiseCharacterDetailSideUpdateRequested(slot.GetCharacter(), slot.FormationCoord.Position);

        if (InputManager.Instance.IsAndroid && !InputManager.Instance.IsUsingController) return;

        //EventSystem.current.SetSelectedGameObject(slot.gameObject);
        //UIEvents.RaiseFormationCharacterSlotUISelected(slot);
        //EventSystem.current.SetSelectedGameObject(defaultSelectableAfterSlot.gameObject);
    }

    #endregion

    #region Buttons

    public void OnButtonPlayClicked()
    {
        if (currentMatch == null) return;

        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        StorySystemManager.Instance.StartMatchBattle(currentMatch, currentNode);
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
        if (currentNode == null)
            DialogEvents.RaiseDialogMenuClosed();
    }

    public void OnToggleAutoBattle(bool boolValue)
    {
        AutoBattleManager.Instance.ToggleAutoBattle();
    }

    #endregion

}
