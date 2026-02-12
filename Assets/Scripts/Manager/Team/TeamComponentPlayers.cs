using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

public class TeamComponentPlayers
{
    public List<CharacterData> FullBattleCharacterDataList { get; private set; }
    public List<Character> FullBattleCharacterList { get; private set; }

    public List<CharacterData> MiniBattleCharacterDataList { get; private set; }
    public List<Character> MiniBattleCharacterList { get; private set; }

    public TeamComponentPlayers(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        FullBattleCharacterDataList = new();
        FullBattleCharacterList = new();
        MiniBattleCharacterDataList = new();
        MiniBattleCharacterList = new();
        
        if (teamSaveData == null) 
        {
            PopulateFromData(FullBattleCharacterDataList, teamData.FullBattleCharacterIds);
            PopulateFromData(MiniBattleCharacterDataList, teamData.MiniBattleCharacterIds);
        }
    }

    private void PopulateFromSaveData(
        List<CharacterData> listCharacter, 
        List<CharacterSaveData> listCharacterSaveData)
    {
        //TODO use guid
    }

    private void PopulateFromData(
        List<CharacterData> listCharacterData, 
        List<string> listIds)
    {
        foreach (string characterId in listIds)
        {
            if (string.IsNullOrEmpty(characterId)) return;

            CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
            if (characterData != null)
                listCharacterData.Add(characterData);
            else
                LogManager.Warning($"[TeamComponentPlayers] CharacterData not found for ID: {characterId}");
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

    public List<Character> GetCharacterList(BattleType battleType)
    {
        return battleType switch
        {
            BattleType.Full => FullBattleCharacterList,
            BattleType.Mini => MiniBattleCharacterList,
            _ => FullBattleCharacterList
        };
    }

    public void ClearCharacterList(BattleType battleType)
    {
        switch (battleType)
        {
            case BattleType.Full:
                FullBattleCharacterList.Clear();
                break;
            case BattleType.Mini:
                MiniBattleCharacterList.Clear();
                break;
            default:
                LogManager.Warning($"[TeamComponentPlayers] Unknown battle type: {battleType}");
                return;
        }
    }

    public int GetCharacterCount(BattleType battleType)
    {
        return GetCharacterList(battleType).Count;
    }

    public bool HasCharacters(BattleType battleType)
    {
        return GetCharacterCount(battleType) > 0;
    }

}
