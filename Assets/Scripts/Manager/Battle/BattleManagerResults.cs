using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.Item;

public class BattleManagerResults
{
    #region Fields

    private BattleResultData battleResultData;
    private SceneGroup sceneWorld;
    private SceneGroup sceneDebugMainMenu;

    private int xpBaseFull = 900;
    private float expLvFactorFull = 100;
    private int xpBaseMini = 100;
    private float expLvFactorMini = 100;

    private int goldBaseFull = 4000;
    private float goldLvFactorFull = 1000;
    private int goldBaseMini = 400;
    private float goldLvFactorMini = 100;

    public BattleResultData BattleResultData => battleResultData; 

    #endregion

    #region Constructor

    public BattleManagerResults(SceneGroup sceneWorld, SceneGroup sceneDebugMainMenu)
    {
        this.battleResultData = new BattleResultData();
        this.sceneWorld = sceneWorld;
        this.sceneDebugMainMenu = sceneDebugMainMenu;
    }

    #endregion

    #region Logic

    public void CreateBattleResultData(
        Dictionary<TeamSide, int> scores,
        Dictionary<TeamSide, Team> teams,
        TeamSide userSide,
        BattleType battleType,
        bool isForfeit)
    {
        battleResultData.Clear();

        battleResultData.BattleResultsType = BattleArgs.BattleResultsType;
        battleResultData.BattleType = battleType;
        battleResultData.UserSide = userSide;
        battleResultData.EnemyLevel = BattleArgs.AwayTeamLevel;

        /*
        battleResultData.WinningSide = scores[TeamSide.Home] == scores[TeamSide.Away]
            ? TeamSide.None
            : scores[TeamSide.Home] > scores[TeamSide.Away]
                ? TeamSide.Home
                : TeamSide.Away;
        */

        if (isForfeit)
            battleResultData.WinningSide = battleResultData.EnemySide;
        else
            battleResultData.WinningSide = scores[TeamSide.Home] > scores[TeamSide.Away] ? TeamSide.Home : TeamSide.Away;

        battleResultData.Scores = new Dictionary<TeamSide, int>(scores);
        battleResultData.Teams = new Dictionary<TeamSide, Team>(teams);

        battleResultData.MatchRank = CalculateMatchRank(battleResultData);

        if (scores[TeamSide.Home] > scores[TeamSide.Away])
            battleResultData.WinningSide = TeamSide.Home;
        else if (scores[TeamSide.Away] > scores[TeamSide.Home])
            battleResultData.WinningSide = TeamSide.Away;

        ApplyBattleRewards(userSide);
        GiveBattleRewards();
        //TryProgresStory();
    }

    public void Clear() 
    {
        battleResultData.Clear();
    }

    private void ApplyBattleRewards(TeamSide userSide)
    {
        if(!IsElegibleForRewards()) return;

        switch (battleResultData.BattleResultsType)
        {

            /*
            case BattleResultsType.MatchNode:
                battleResultData.XpReward = (int)(xpBaseFull + (expLvFactorFull * battleResultData.Teams[battleResultData.EnemySide].Level));
                battleResultData.GoldReward = (int)(goldBaseFull + (goldLvFactorFull * battleResultData.Teams[battleResultData.EnemySide].Level));
                battleResultData.ItemRewards = GetMatchNodeRewards();
                break;

            case BattleResultsType.Encounter:
                battleResultData.XpReward = (int)(xpBaseMini + (expLvFactorMini * battleResultData.Teams[battleResultData.EnemySide].Level));
                battleResultData.GoldReward = (int)(goldBaseMini + (goldLvFactorMini * battleResultData.Teams[battleResultData.EnemySide].Level));
                battleResultData.ItemRewards = GetEncounterDrops();
                break;
            */

            case BattleResultsType.MatchNode:
                battleResultData.XpReward = (int)(xpBaseFull + (expLvFactorFull * battleResultData.EnemyLevel));
                battleResultData.GoldReward = (int)(goldBaseFull + (goldLvFactorFull * battleResultData.EnemyLevel));
                battleResultData.ItemRewards = GetMatchNodeRewards();
                break;

            case BattleResultsType.Encounter:
                battleResultData.XpReward = (int)(xpBaseMini + (expLvFactorMini * battleResultData.EnemyLevel));
                battleResultData.GoldReward = (int)(goldBaseMini + (goldLvFactorMini * battleResultData.EnemyLevel));
                battleResultData.ItemRewards = GetEncounterDrops();
                break;
        }
    }

    public void GiveBattleRewards()
    {
        if(!IsElegibleForRewards()) return;

        if(!string.IsNullOrEmpty(BattleArgs.MatchChainNodeId)) 
            MatchEvents.RaiseMatchChainNodeMatchCompleted(
                StorySystemManager.Instance.GetMatchChainNode<MatchChainNodeMatch>(BattleArgs.MatchChainNodeId), 
                battleResultData.MatchRank);

        foreach (var itemReward in battleResultData.ItemRewards) 
        {
            ItemManager.Instance.AddItem(
                ItemFactory.CreateById(
                    itemReward.ItemId), 
                    itemReward.Quantity);
        }

        ItemManager.Instance.Add(CurrencyType.Gold, battleResultData.GoldReward);

        GiveXP();
    }

    public void TryProgresStory()
    {
        if(!battleResultData.IsUserWin) return;
        if(battleResultData.BattleResultsType != BattleResultsType.MatchStory) return;
        if(BattleArgs.MatchId == null) return;

        /*
        set flag battleResultData.MatchId

        */
    }

    #endregion

    #region Helpers

    private bool IsElegibleForRewards() 
    {
        if(!battleResultData.IsUserWin) return false;

        switch (BattleArgs.BattleResultsType) 
        {
            case BattleResultsType.MatchStory:
            case BattleResultsType.MatchNode:
            case BattleResultsType.Encounter:
                return true;
            case BattleResultsType.Arcade:
            default:
                return false;
        }
    }

    private List<ItemReward> GetEncounterDrops()
    {
        var rewards = new List<ItemReward>();

        foreach (var drop in BattleArgs.EncounterData.Drops)
        {
            if (Random.value <= drop.DropChance)
            {
                int quantity = Random.Range(drop.QuantityMin, drop.QuantityMax + 1);
                rewards.Add(new ItemReward
                {
                    ItemId = drop.ItemId,
                    Quantity = 1
                });
            }
        }
        
        return rewards;
    }

    private List<ItemReward> GetMatchNodeRewards()
    {
        return StorySystemManager.Instance.GetMatchChainNode<MatchChainNodeMatch>(BattleArgs.MatchChainNodeId).GetRewardsByRank(battleResultData.MatchRank);
        //return new List<ItemReward>();
    }

    public MatchRank CalculateMatchRank(BattleResultData data)
    {
        // Calculate rank based on score difference, performance, etc

        if(!data.IsUserWin) return MatchRank.None;

        int scoreDifference = battleResultData.GoalDifference;
        
        if (scoreDifference >= 5) return MatchRank.S;
        if (scoreDifference >= 3) return MatchRank.A;
        if (scoreDifference >= 1) return MatchRank.B;
        return MatchRank.None;
    }

    private void GiveXP()
    {
        if (TeamManager.Instance?.ActiveLoadout == null) return;

        foreach (string guid in battleResultData.Teams[battleResultData.UserSide].GetCharacterGuids(battleResultData.BattleType))
        {
            BattleResultDataXp result = new();

            var character = CharacterManager.Instance.GetCharacter(guid);

            result.Character = character;

            result.StartLevel = character.Level;
            result.StartXp = character.CurrentXp;
            result.StartXpToNextLevel = character.XpToNextLevel;

            character.AddXp(battleResultData.XpReward);

            result.EndLevel = character.Level;
            result.EndXp = character.CurrentXp;
            result.XPGained = battleResultData.XpReward;
            result.EndXpToNextLevel = character.XpToNextLevel;

            battleResultData.XpResult.Add(result);
        }
    }

    #endregion

    #region Events

    public void Subscribe() 
    {
        BattleEvents.OnResultsContinueRequested += HandleResultsContinueRequested;
    }

    public void Unsubscribe()
    {
        BattleEvents.OnResultsContinueRequested += HandleResultsContinueRequested;
    }

    private void HandleResultsContinueRequested()
    {
        switch(battleResultData.BattleResultsType) 
        {
            case BattleResultsType.Debug:
                SceneLoader.Instance.LoadGroup(sceneDebugMainMenu);
                break;
            case BattleResultsType.Arcade:
                LogManager.Trace("continue arcade");
                break;
            default:
                SceneLoader.Instance.LoadGroup(sceneWorld);
                break;
        }
    }

    #endregion

}
