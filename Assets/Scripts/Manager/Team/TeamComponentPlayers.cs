using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

public class TeamComponentPlayers
{
    // Full Battle Team
    public List<CharacterData> FullBattleCharacterDataList { get; private set; }
    public List<CharacterEntityBattle> FullBattleCharacterEntities { get; private set; }
    public List<string> FullBattleCharacterGuids { get; private set; }

    // Mini Battle Team
    public List<CharacterData> MiniBattleCharacterDataList { get; private set; }
    public List<CharacterEntityBattle> MiniBattleCharacterEntities { get; private set; }
    public List<string> MiniBattleCharacterGuids { get; private set; }

    public TeamComponentPlayers(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        // Initialize Full Battle lists
        FullBattleCharacterDataList = new();
        FullBattleCharacterEntities = new();
        FullBattleCharacterGuids = new();
        
        // Initialize Mini Battle lists
        MiniBattleCharacterDataList = new();
        MiniBattleCharacterEntities = new();
        MiniBattleCharacterGuids = new();
        
        if (teamSaveData == null) 
        {
            if (teamData != null) 
            {
                PopulateFromData(FullBattleCharacterDataList, teamData.FullBattleCharacterIds);
                PopulateFromData(MiniBattleCharacterDataList, teamData.MiniBattleCharacterIds);                
            }
        }
        else
        {
            PopulateFromSaveData(
                FullBattleCharacterDataList, 
                FullBattleCharacterGuids);
            
            PopulateFromSaveData(
                MiniBattleCharacterDataList, 
                MiniBattleCharacterGuids);
        }
    }

    private void PopulateFromSaveData(
        List<CharacterData> characterDataList, 
        List<string> characterGuidList)
    {
        /*
        foreach (CharacterSaveData saveData in characterSaveDataList)
        {
            characterGuidList.Add(saveData.CharacterGuid);

            CharacterData characterData = CharacterManager.Instance.GetCharacterData(saveData.CharacterId);
            if (characterData != null)
                characterDataList.Add(characterData);
        }
        */
    }

    private void PopulateFromData(
        List<CharacterData> characterDataList, 
        List<string> characterIdList)
    {
        foreach (string characterId in characterIdList)
        {
            if (string.IsNullOrEmpty(characterId)) continue;

            CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
            if (characterData != null)
            {
                characterDataList.Add(characterData);
            }
            else
            {
                LogManager.Warning($"[TeamComponentPlayers] CharacterData not found for ID: {characterId}");
            }
        }
    }

    public List<CharacterData> GetCharacterDataList(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleCharacterDataList,
            BattleType.Mini => MiniBattleCharacterDataList,
            _ => FullBattleCharacterDataList
        };
    }

    public List<CharacterEntityBattle> GetCharacterEntities(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleCharacterEntities,
            BattleType.Mini => MiniBattleCharacterEntities,
            _ => FullBattleCharacterEntities
        };
    }

    public List<string> GetCharacterGuids(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleCharacterGuids,
            BattleType.Mini => MiniBattleCharacterGuids,
            _ => FullBattleCharacterGuids
        };
    }

    public void SetCharacterGuid(BattleType battleType, int slotIndex, string characterGuid)
    {
        List<string> guids = GetCharacterGuids(battleType);
        
        while (guids.Count <= slotIndex)
            guids.Add(null);

        guids[slotIndex] = characterGuid;
    }

    public void RemoveCharacterGuid(BattleType battleType, string characterGuid)
    {
        List<string> guids = GetCharacterGuids(battleType);
        int index = guids.IndexOf(characterGuid);
        if (index >= 0)
            guids[index] = null;
    }

    public void ClearCharacterEntities(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.Full:
                FullBattleCharacterEntities.Clear();
                break;
            case BattleType.Mini:
                MiniBattleCharacterEntities.Clear();
                break;
            default:
                LogManager.Warning($"[TeamComponentPlayers] Unknown battle type: {battleType}");
                break;
        }
    }

    public void ClearAll(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.Full:
                FullBattleCharacterDataList.Clear();
                FullBattleCharacterEntities.Clear();
                FullBattleCharacterGuids.Clear();
                break;
            case BattleType.Mini:
                MiniBattleCharacterDataList.Clear();
                MiniBattleCharacterEntities.Clear();
                MiniBattleCharacterGuids.Clear();
                break;
            default:
                LogManager.Warning($"[TeamComponentPlayers] Unknown battle type: {battleType}");
                break;
        }
    }

    public int GetCharacterDataCount(BattleType battleType)
    {
        return GetCharacterDataList(battleType).Count;
    }

    public int GetCharacterEntityCount(BattleType battleType)
    {
        return GetCharacterEntities(battleType).Count;
    }

    public bool HasCharacterData(BattleType battleType)
    {
        return GetCharacterDataCount(battleType) > 0;
    }

    public bool HasCharacterEntities(BattleType battleType)
    {
        return GetCharacterEntityCount(battleType) > 0;
    }
}
