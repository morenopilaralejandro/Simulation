using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Localization;

public class Team
{
    [SerializeField] private string teamId;
    public string TeamId => teamId;

    [SerializeField] private ComponentLocalization localizationComponent;

    [SerializeField] private Formation formation;
    public Formation Formation => formation;

    [SerializeField] private Kit kit;
    public Kit Kit => kit;

    [SerializeField] private int lv;
    public int Lv => lv;

    [SerializeField] private List<CharacterData> characterDataList = new();
    public List<CharacterData> CharacterDataList => characterDataList;

    [SerializeField] private List<Character> characters = new();
    public List<Character> Characters => characters;

    public void Initialize(TeamData teamData)
    {
        teamId = teamData.TeamId;

        localizationComponent = new ComponentLocalization();
        localizationComponent.Initialize(
            LocalizationEntity.Team,
            teamData.TeamId,
            new [] { LocalizationField.Name }
        );

        formation = FormationManager.Instance.GetFormation(teamData.FormationId);
        kit = KitManager.Instance.GetKit(teamData.KitId);
        lv = teamData.Lv;

        characterDataList.Clear();
        foreach (var characterId in teamData.CharacterIds)
        {
            if (!string.IsNullOrEmpty(characterId))
            {
                CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
                if (characterData != null)
                    characterDataList.Add(characterData);
                else
                    LogManager.Warning($"[Team] CharacterData not found for ID: {characterId}");
            }
        }
    }

    public string GetTeamName() => localizationComponent.GetString(LocalizationField.Name);
}
