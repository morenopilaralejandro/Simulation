using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class Team : MonoBehaviour
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

    [SerializeField] private bool isRomazed = false;
    [SerializeField] private string stringTableNameLocalized = "TeamNamesLocalized";
    [SerializeField] private string stringTableNameRomanized = "TeamNamesRomanized"; 

    /*
    public void Initialize(TeamData teamData)
    {
        teamId = teamData.TeamId;
        formation = TeamManager.Instance.GetFormationById(teamData.FormationId);
        kit = KitManager.Instance.GetKitById(teamData.KitId);
        lv = teamData.Lv;

        characterDataList.Clear();
        foreach (var characterId in teamData.CharacterIds)
        {
            CharacterData characterData = CharacterManager.Instance.GetCharacterDataById(characterId);
            if (characterData != null)
                characterDataList.Add(characterData);
            else
                LogManager.Warning($"CharacterData not found for ID: {characterId}", this);
        }
    }
    */

    private void SetName()
    {
        localizedName = new LocalizedString(
            isRomazed ? stringTableNameRomanized : stringTableNameLocalized,
            teamId
        );
    }
}
