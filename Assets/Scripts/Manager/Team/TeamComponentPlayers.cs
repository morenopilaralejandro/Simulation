using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;

public class TeamComponentPlayers
{
    public List<CharacterData> CharacterDataList { get; private set; }
    public List<Character> CharacterList { get; private set; }

    public TeamComponentPlayers(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        CharacterDataList.Clear();
        foreach (var characterId in teamData.CharacterIds)
        {
            if (!string.IsNullOrEmpty(characterId))
            {
                CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
                if (characterData != null)
                    CharacterDataList.Add(characterData);
                else
                    LogManager.Warning($"[TeamComponentPlayers] CharacterData not found for ID: {characterId}");
            }
        }
    }
}
