using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

/*
    data used for enemy teams from csv
    entity used during battle
    character used for team menu
    guids used for user team persistence
*/

public class TeamComponentPlayers
{
    // Full Battle Team
    public List<CharacterData> FullBattleCharacterDataList { get; private set; }
    public List<CharacterEntityBattle> FullBattleCharacterEntities { get; private set; }
    public List<Character> FullBattleCharacters { get; private set; }
    public List<string> FullBattleCharacterGuids { get; private set; }

    // Mini Battle Team
    public List<CharacterData> MiniBattleCharacterDataList { get; private set; }
    public List<CharacterEntityBattle> MiniBattleCharacterEntities { get; private set; }
    public List<Character> MiniBattleCharacters { get; private set; }
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
        FullBattleCharacters = new();
        FullBattleCharacterGuids = new();
        
        // Initialize Mini Battle lists
        MiniBattleCharacterDataList = new();
        MiniBattleCharacterEntities = new();
        MiniBattleCharacters = new();
        MiniBattleCharacterGuids = new();
        
        if (teamSaveData == null) 
        {
            PopulateFromData(FullBattleCharacterDataList, teamData.FullBattleCharacterIds);
            PopulateFromData(MiniBattleCharacterDataList, teamData.MiniBattleCharacterIds);  
        }
        else
        {
            PopulateFromSaveData(FullBattleCharacterGuids, teamSaveData.CustomFullBattleCharacterGuids);
            PopulateFromSaveData(MiniBattleCharacterGuids, teamSaveData.CustomMiniBattleCharacterGuids);
        }
    }

    private void PopulateFromSaveData(
        List<string> characterGuidList, 
        List<string> customCharacterGuidList)
    {
        if (customCharacterGuidList == null) return;
        characterGuidList.AddRange(customCharacterGuidList);
    }

    private void PopulateFromData(
        List<CharacterData> characterDataList, 
        List<string> characterIdList)
    {
        foreach (string characterId in characterIdList)
        {
            if (string.IsNullOrEmpty(characterId)) continue;

            CharacterData characterData = CharacterDatabase.Instance.GetCharacterData(characterId);
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

    public List<Character> GetCharacters(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleCharacters,
            BattleType.Mini => MiniBattleCharacters,
            _ => FullBattleCharacters
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

    public void SetCharacter(BattleType battleType, int slotIndex, Character character)
    {
        List<Character> characters = GetCharacters(battleType);
        
        while (characters.Count <= slotIndex)
            characters.Add(null);

        characters[slotIndex] = character;
    }

    public void SetCharacterEntity(BattleType battleType, int slotIndex, CharacterEntityBattle characterEntityBattle)
    {
        List<CharacterEntityBattle> entities = GetCharacterEntities(battleType);
        
        while (entities.Count <= slotIndex)
            entities.Add(null);

        entities[slotIndex] = characterEntityBattle;
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

    public void ClearCharacters(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.Full:
                FullBattleCharacters.Clear();
                break;
            case BattleType.Mini:
                MiniBattleCharacters.Clear();
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
                FullBattleCharacters.Clear();
                FullBattleCharacterGuids.Clear();
                break;
            case BattleType.Mini:
                MiniBattleCharacterDataList.Clear();
                MiniBattleCharacterEntities.Clear();
                MiniBattleCharacters.Clear();
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

    public CharacterEntityBattle GetEntityByGuid(string characterGuid, BattleType battleType)
    {
        List<CharacterEntityBattle> entities = GetCharacterEntities(battleType);
        int count = entities.Count;
        
        for (int i = 0; i < count; i++)
        {
            if (entities[i].CharacterGuid == characterGuid)
                return entities[i];
        }
        
        return null;
    }

    public Character GetCharacterByGuid(string characterGuid, BattleType battleType)
    {
        List<Character> characters = GetCharacters(battleType);
        int count = characters.Count;
        
        for (int i = 0; i < count; i++)
        {
            if (characters[i].CharacterGuid == characterGuid)
                return characters[i];
        }
        
        return null;
    }

    public bool ContainsCharacterGuid(string characterGuid, BattleType battleType)
    {
        if (string.IsNullOrEmpty(characterGuid)) return false;
        return GetCharacterGuids(battleType).Contains(characterGuid);
    }
}
