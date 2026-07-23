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

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;

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
        PopulateTeamWithCharacters(team, currentMatch.BattleType);
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
        EventSystem.current.SetSelectedGameObject(defaultSelectableAfterSlot.gameObject);
    }

    #endregion

    #region Buttons

    public void OnButtonPlayClicked()
    {
        if (currentMatch == null) return;

        StartMatchBattle(currentMatch);
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
    }

    public void OnToggleAutoBattle(bool boolValue)
    {
        AutoBattleManager.Instance.ToggleAutoBattle();
    }

    #endregion

    #region Battle

    private void PopulateTeamWithCharacters(Team team, BattleType currentType)
    {
        team.ClearCharacters(currentType);

        var dataList = team.GetCharacterDataList(currentType);
        var characters = team.GetCharacters(currentType);

        for (int i = 0; i < dataList.Count; i++)
        {
            var data = dataList[i];

            var character = new Character(data);
            character.SetLevel(character.MaxLevel);
            character.TryEquipWingDefault();
            character.ScaleDifficultySystem();

            characters.Add(character);
        }
    }

    private async void StartMatchBattle(Match match)
    {
        var worldManager = WorldManager.Instance;
        var player = WorldManager.Instance.PlayerWorldEntity;

        worldManager.SetIsTransitioning(true);
        player.SetControlEnabled(false);

        DialogManager.Instance.ForceEndDialog();

        WorldArgs.Set(
            zoneId: worldManager.CurrentZone != null ? worldManager.CurrentZone.zoneId : null,
            realm: worldManager.CurrentRealm,
            playerPosition: player.CurrentTilePosition3d(),
            facingDirection: player.FacingToVector(player.FacingDirection),
            worldState: WorldState.InEncounter,
            hour: worldManager.CurrentHour
        );

        await worldManager.FadeOut();

        if (worldManager.CurrentZone != null && worldManager.CurrentZone.zoneType == ZoneType.Overworld)
            await ChunkStreamingManager.Instance.StopStreaming();

        bool unloadSuccess = await worldManager.UnloadCurrentZone();
        worldManager.SetState(WorldState.InEncounter);

        BattleArgs.SetFull(
            homeTeamGuid: TeamManager.Instance.ActiveLoadoutGuid,
            awayTeamId: match.TeamId,
            battleResultsType: BattleResultsType.MatchNode,
            timeOfDay: worldManager.CurrentTimeOfDay,
            ballId: match.BallId,
            bgmId: match.BgmId,
            fieldId: match.FieldId
        );

        SceneLoader.Instance.LoadGroup(sceneBattle);
    }

    #endregion
}
