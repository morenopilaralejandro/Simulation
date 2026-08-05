using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.World;

public class StorySystemMatchChain
{
    #region Fields

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;

    private MatchData auxMatchData;
    private TeamData auxTeamData;
    private MatchChainNodeData auxNodeData;

    private Dictionary<string, MatchChain> dict = new Dictionary<string, MatchChain>();

    #endregion

    #region Constructor

    public StorySystemMatchChain(SceneGroup sceneBattle) 
    {
        this.sceneBattle = sceneBattle;
    }

    public void InitializeFromDatabase()
    {
        foreach (MatchChainData data in DatabaseManager.Instance.DatabaseRegistry.MatchChainData.Data.Values)
        {
            dict[data.MatchChainId] = MatchChainFactory.Create(data);
        }
    }

    #endregion
   
    #region Peristance

    public void Import(StorySystemSaveData saveData)
    {
        foreach(var chainSaveData in saveData.MatchChainSystemSaveData.MatchChains) 
        {
            dict[chainSaveData.MatchChainId].Import(chainSaveData);
        }
    }

    public MatchChainSystemSaveData Export() 
    {
        MatchChainSystemSaveData saveData = new MatchChainSystemSaveData();
        List<MatchChainSaveData> list = new ();
        foreach(var chain in dict.Values) 
        {
            list.Add(chain.Export());
        }
        saveData.MatchChains = list;
        return saveData;
    }

    #endregion

    #region MatchChainNode

    public string GetTeamEmblemAddressByMatchId(string matchId) 
    {
        auxMatchData = DatabaseManager.Instance.GetMatchData(matchId);
        auxTeamData = DatabaseManager.Instance.GetTeamData(auxMatchData.TeamId);
        return AddressableLoader.GetTeamEmblemAddress(auxTeamData.EmblemId);
    }

    public MatchChainNode TryGetNextNode(string sourceNodeId)
    {
        auxNodeData = DatabaseManager.Instance.GetMatchChainNodeData(sourceNodeId);

        if (auxNodeData == null) return null;

        return dict[auxNodeData.MatchChainId].GetNodeByIndex(auxNodeData.NodeIndex + 1);
    }

    public MatchChainNode GetMatchChainNode(string sourceNodeId)
    {
        auxNodeData = DatabaseManager.Instance.GetMatchChainNodeData(sourceNodeId);
        return dict[auxNodeData.MatchChainId].GetNodeById(sourceNodeId);
    }

    public T GetMatchChainNode<T>(string id) where T : MatchChainNode
    {
        auxNodeData = DatabaseManager.Instance.GetMatchChainNodeData(id);
        if (dict[auxNodeData.MatchChainId].GetNodeById(id) is T typed) return typed;
        return null;
    }

    public void TryUnlockNextNode(string sourceNodeId)
    {
        TryGetNextNode(sourceNodeId)?.Unlock();
    }

    public MatchChain GetMatchChain(string matchChainId)
    {
        return dict.TryGetValue(matchChainId, out var chain) ? chain : null;
    }

    public void TrySetSelectedIndex(MatchChainNode node) 
    {
        dict[node.MatchChainId].SetSelectedIndex(node.NodeIndex);
    }

    #endregion

    #region Battle

    public void PopulateTeamWithCharacters(Team team, BattleType currentType, int level)
    {
        team.ClearCharacters(currentType);

        var dataList = team.GetCharacterDataList(currentType);
        var characters = team.GetCharacters(currentType);

        for (int i = 0; i < dataList.Count; i++)
        {
            var data = dataList[i];

            var character = new Character(data);
            character.SetLevel(level);
            character.TryEquipWingDefault();
            character.ScaleDifficultySystem();

            characters.Add(character);
        }
    }

    public async Task StartMatchBattle(Match match, MatchChainNodeMatch matchChainNodeMatch)
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
            battleResultsType: matchChainNodeMatch != null ? BattleResultsType.MatchNode : BattleResultsType.MatchStory,
            timeOfDay : match.HasTimeOfDayRestriction ? match.TimeOfDay : worldManager.CurrentTimeOfDay,
            ballId: match.BallId,
            bgmId: match.BgmId,
            fieldId: match.FieldId,
            matchChainNodeId: matchChainNodeMatch != null ? matchChainNodeMatch.MatchChainNodeId : null,
            awayTeamLevel : match.Level
        );

        SceneLoader.Instance.LoadGroup(sceneBattle);
    }

    #endregion


    #region Events

    public void Subscribe() 
    {
        MatchEvents.OnMatchChainNodeMatchCompleted += HandleMatchChainNodeMatchCompleted;
    }

    public void Unsubscribe() 
    {
        MatchEvents.OnMatchChainNodeMatchCompleted -= HandleMatchChainNodeMatchCompleted;
    }

    private void HandleMatchChainNodeMatchCompleted(MatchChainNodeMatch node, MatchRank matchRank)
    {
        node.SetMatchRankBest(matchRank);
        node.Complete();
    }

    #endregion
}
