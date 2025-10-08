using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;

public class TeamComponentPlayers
{
    private List<CharacterData> characterDataList = new();
    private List<Character> characterList = new();

    public List<CharacterData> CharacterDataList => characterDataList;
    public List<Character> CharacterList => characterList;

    public TeamComponentPlayers(TeamData teamData)
    {
        Initialize(teamData);
    }

    public void Initialize(TeamData teamData)
    {
        characterDataList.Clear();
        foreach (var characterId in teamData.CharacterIds)
        {
            if (!string.IsNullOrEmpty(characterId))
            {
                CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
                if (characterData != null)
                    characterDataList.Add(characterData);
                else
                    LogManager.Warning($"[TeamComponentPlayers] CharacterData not found for ID: {characterId}");
            }
        }
    }
}
