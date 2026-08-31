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
    private SceneGroup sceneCredits;

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

    public BattleManagerResults(SceneGroup sceneWorld, SceneGroup sceneDebugMainMenu, SceneGroup sceneCredits)
    {
        this.battleResultData = new BattleResultData();
        this.sceneWorld = sceneWorld;
        this.sceneDebugMainMenu = sceneDebugMainMenu;
        this.sceneCredits = sceneCredits;
    }

    #endregion

    #region Logic

    public void CreateBattleResultData(
        Dictionary<TeamSide, int> scores,
        Dictionary<TeamSide, Team> teams,
        TeamSide userSide,
        BattleType battleType,
        TeamSide? forcedWinner = null)
    {
        battleResultData.Clear();

        battleResultData.BattleResultsType = BattleArgs.BattleResultsType;
        battleResultData.BattleType = battleType;
        battleResultData.UserSide = userSide;
        battleResultData.EnemyLevel = BattleArgs.AwayTeamLevel;

        battleResultData.IsForcedWinner = forcedWinner.HasValue;

        if (battleResultData.IsForcedWinner)
            battleResultData.WinningSide = forcedWinner.Value;
        else
            battleResultData.WinningSide = scores[TeamSide.Home] > scores[TeamSide.Away] ? TeamSide.Home : TeamSide.Away;

        battleResultData.Scores = new Dictionary<TeamSide, int>(scores);
        battleResultData.Teams = new Dictionary<TeamSide, Team>(teams);

        battleResultData.MatchRank = CalculateMatchRank(battleResultData);

        ApplyBattleRewards(userSide);
        GiveBattleRewards();
        TryProgresStory();
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

        StorySystemManager.Instance.SetFlag(BattleArgs.MatchId, true);

        switch (BattleArgs.MatchId) 
        {
            /*
            case "match-00021-boss_0":
            case "match-00022-boss_1":
            case "match-00023-boss_2":
            case "match-00024-boss_3":
                break;
            */

            case "match-00025-boss_4":
                StorySystemManager.Instance.SetFlag("clear_game", true);
                StorySystemManager.Instance.SetFlag("pending_ending", true);

                if (ItemManager.Instance.HasItem(ItemFactory.CreateById("item-important-00005-shard_joy")) && 
                    ItemManager.Instance.HasItem(ItemFactory.CreateById("item-important-00006-shard_love")))
                {
                    StorySystemManager.Instance.SetFlag("ending_good", true);
                } else 
                {
                    StorySystemManager.Instance.SetFlag("ending_good", false);
                }

                break;

        }
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

        if (StorySystemManager.Instance.GetFlag("pending_ending") && 
            !StorySystemManager.Instance.GetFlag("ending_good")) 
        {
            StorySystemManager.Instance.SetFlag("pending_ending", false);
            StorySystemManager.Instance.SetFlag("pending_starting_spawn", true);
            StorySystemManager.Instance.SetFlag("allow_quick_travel", true);
            SceneLoader.Instance.LoadGroup(sceneCredits);
            return;
        }

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
