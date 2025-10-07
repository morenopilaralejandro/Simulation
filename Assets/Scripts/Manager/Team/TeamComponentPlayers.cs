using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;

public class TeamComponentPlayers : MonoBehaviour
{
    [SerializeField] private List<CharacterData> characterDataList = new();
    [SerializeField] private List<Character> characterList = new();

    public List<CharacterData> CharacterDataList => characterDataList;
    public List<Character> CharacterList => characterList;

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
