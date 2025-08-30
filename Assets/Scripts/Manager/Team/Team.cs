using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Simulation.Enums.Localization;

public class Team
{
    [SerializeField] private string teamId;
    public string TeamId => teamId;

    [SerializeField] private LocalizedString localizedName;
    public LocalizedString LocalizedName => localizedName;

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
        formation = FormationManager.Instance.GetFormation(teamData.FormationId);
        kit = KitManager.Instance.GetKit(teamData.KitId);
        lv = teamData.Lv;

        characterDataList.Clear();
        foreach (var characterId in teamData.CharacterIds)
        {
            if (characterId != "")
            {
                CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
                if (characterData != null)
                    characterDataList.Add(characterData);
                else
                    LogManager.Warning($"[Team] CharacterData not found for ID: {characterId}");
            }
        }

        SetName();
    }

    private void SetName()
    {
        localizedName = new LocalizedString(
            LocalizationManager.Instance.GetTableReference(LocalizationEntity.Team, LocalizationField.Name),
            teamId
        );
    }
}
