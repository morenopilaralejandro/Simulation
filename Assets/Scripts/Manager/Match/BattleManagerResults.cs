using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Match;

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
        this.sceneWorld = sceneWorld;
        this.sceneDebugMainMenu = sceneDebugMainMenu;
    }

    #endregion

    #region Logic

    public void CreateBattleResultData(
        int homeScore, 
        int awayScore, 
        int enemyLv,
        TeamSide userSide)
    {
        battleResultData.Clear();

        battleResultData.BattleResultsType = BattleArgs.BattleResultsType;
        battleResultData.EnemyLv = enemyLv;
        battleResultData.WinningSide = homeScore > awayScore ? TeamSide.Home : TeamSide.Away;
        battleResultData.HomeScore = homeScore;
        battleResultData.AwayScore = awayScore;
        battleResultData.FinalScore = new Dictionary<TeamSide, int>
        {
            { TeamSide.Home, homeScore },
            { TeamSide.Away, awayScore }
        };

        // Calculate and apply rewards
        ApplyBattleRewards(userSide);
    }

    private void ApplyBattleRewards(TeamSide userSide)
    {
        bool playerWon = battleResultData.WinningSide == userSide;
        
        if (!playerWon) return;

        if (battleResultData.BattleResultsType == BattleResultsType.Match)
        {
            battleResultData.ExpReward = (int) (xpBaseFull + (expLvFactorFull * battleResultData.EnemyLv));
            battleResultData.GoldReward = (int) (goldBaseFull + (goldLvFactorFull * battleResultData.EnemyLv));
            battleResultData.ItemRewards = GetMatchRewards(playerWon);
            battleResultData.MatchRank = CalculateMatchRank(battleResultData);
        }
        else if (battleResultData.BattleResultsType == BattleResultsType.Encounter)
        {
            battleResultData.ExpReward = (int) (xpBaseMini + (expLvFactorMini * battleResultData.EnemyLv));
            battleResultData.GoldReward = (int) (goldBaseMini + (goldLvFactorMini * battleResultData.EnemyLv));
            battleResultData.ItemRewards = GetEncounterDrops(playerWon);
        }
    }

    private void GiveBattleRewards()
    {

            if(!string.IsNullOrEmpty(BattleArgs.MatchChainNodeId)) 
                MatchEvents.RaiseMatchChainNodeMatchCompleted(
                    StorySystemManager.Instance.GetMatchChainNode<MatchChainNodeMatch>(BattleArgs.MatchChainNodeId), 
                    battleResultData.MatchRank);

    }

    #endregion

    #region Helpers

    private List<ItemReward> GetEncounterDrops(bool won)
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

    private List<ItemReward> GetMatchRewards(bool won)
    {
        return StorySystemManager.Instance.GetMatchChainNode<MatchChainNodeMatch>(BattleArgs.MatchChainNodeId).GetRewardsByRank(battleResultData.MatchRank);
        //return new List<ItemReward>();
    }

    private MatchRank CalculateMatchRank(BattleResultData data)
    {
        // Calculate rank based on score difference, performance, etc
        int scoreDifference = Mathf.Abs(data.HomeScore - data.AwayScore);
        
        if (scoreDifference >= 5) return MatchRank.S;
        if (scoreDifference >= 3) return MatchRank.A;
        if (scoreDifference >= 1) return MatchRank.B;
        return MatchRank.None;
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
                GiveBattleRewards();
                SceneLoader.Instance.LoadGroup(sceneWorld);
                break;
        }

        //check args match id for story completion
    }

    #endregion

}
