using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Localization;

public class Team
{
    #region Components
    private TeamComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private TeamComponentAppearance appearanceComponent;
    private TeamComponentFormation formationComponent;
    private TeamComponentKit kitComponent;
    private TeamComponentLevels levelsComponent;
    private TeamComponentPlayers playersComponent;
    private TeamComponentSide sideComponent;
    private TeamComponentCustomLoadout customLoadoutComponent;
    private TeamComponentPersistence persistenceComponent;
    #endregion

    #region Initialize
    public Team(TeamData teamData, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, teamSaveData);
    }

    public void Initialize(TeamData teamData, TeamSaveData teamSaveData = null)
    {
        attributesComponent = new TeamComponentAttributes(teamData, this, teamSaveData);

        if (teamData != null) 
        {
            localizationStringComponent = new LocalizationComponentString(
                LocalizationEntity.Team,
                teamData.TeamId,
                new [] { LocalizationField.Name }
            );
        }

        appearanceComponent = new TeamComponentAppearance(teamData, this, teamSaveData);
        formationComponent = new TeamComponentFormation(teamData, this, teamSaveData);
        kitComponent = new TeamComponentKit(teamData, this, teamSaveData);
        levelsComponent = new TeamComponentLevels(teamData, this);
        playersComponent = new TeamComponentPlayers(teamData, this, teamSaveData);
        sideComponent = new TeamComponentSide(teamData, this);
        customLoadoutComponent = new TeamComponentCustomLoadout(teamData, this, teamSaveData);
        persistenceComponent = new TeamComponentPersistence(teamData, this);
    }

    public void Deinitialize()
    {
        sideComponent.Deinitialize();
    }
    #endregion

    #region API
    //attributeComponent
    public string TeamId => attributesComponent.TeamId;
    public string TeamGuid => attributesComponent.TeamGuid;
    //localizationComponent
    public string TeamName => 
        IsCustomLoadout ? 
            CustomName : 
            localizationStringComponent.GetString(LocalizationField.Name);
    //appearanceComponent
    public Sprite TeamCrestSprite => appearanceComponent.TeamCrestSprite;
    public string TeamCrestId => appearanceComponent.TeamCrestId;
    public void UpdateAppeariance(string teamCrestId) => appearanceComponent.UpdateAppeariance(teamCrestId); 
    //formationComponent
    public Formation FullBattleFormation => formationComponent.FullBattleFormation;
    public Formation MiniBattleFormation => formationComponent.MiniBattleFormation;
    public Formation GetFormation(BattleType battleType) => formationComponent.GetFormation(battleType);
    public void SetFormation(Formation formation, BattleType battleType) => formationComponent.SetFormation(formation, battleType);
    //kitComponent
    public Kit Kit => kitComponent.Kit;
    public void SetKit(Kit kit) => kitComponent.SetKit(kit);
    //levelsComponent
    public int Level => levelsComponent.Level;
    public void SetLevel(int level) => levelsComponent.SetLevel(level);
    //playersComponent
    public List<CharacterData> FullBattleCharacterDataList => playersComponent.FullBattleCharacterDataList;
    public List<CharacterEntityBattle> FullBattleCharacterEntities => playersComponent.FullBattleCharacterEntities;
    public List<string> FullBattleCharacterGuids => playersComponent.FullBattleCharacterGuids;
    public List<CharacterData> MiniBattleCharacterDataList => playersComponent.MiniBattleCharacterDataList;
    public List<CharacterEntityBattle> MiniBattleCharacterEntities => playersComponent.MiniBattleCharacterEntities;
    public List<string> MiniBattleCharacterGuids => playersComponent.MiniBattleCharacterGuids;
    public List<CharacterData> GetCharacterDataList(BattleType battleType) => playersComponent.GetCharacterDataList(battleType);
    public List<CharacterEntityBattle> GetCharacterEntities(BattleType battleType) => playersComponent.GetCharacterEntities(battleType);
    public void SetCharacterEntity(BattleType battleType, int slotIndex, CharacterEntityBattle characterEntityBattle) => playersComponent.SetCharacterEntity(battleType, slotIndex, characterEntityBattle);
    public List<string> GetCharacterGuids(BattleType battleType) => playersComponent.GetCharacterGuids(battleType);
    public void SetCharacterGuid(BattleType battleType, int slotIndex, string characterGuid) => playersComponent.SetCharacterGuid(battleType, slotIndex, characterGuid);
    public void RemoveCharacterGuid(BattleType battleType, string characterGuid) => playersComponent.RemoveCharacterGuid(battleType, characterGuid);
    public void ClearCharacterEntities(BattleType battleType) => playersComponent.ClearCharacterEntities(battleType);
    public void ClearAll(BattleType battleType) => playersComponent.ClearAll(battleType);
    public int GetCharacterDataCount(BattleType battleType) => playersComponent.GetCharacterDataCount(battleType);
    public int GetCharacterEntityCount(BattleType battleType) => playersComponent.GetCharacterEntityCount(battleType);
    public bool HasCharacterData(BattleType battleType) => playersComponent.HasCharacterData(battleType);
    public bool HasCharacterEntities(BattleType battleType) => playersComponent.HasCharacterEntities(battleType);
    public CharacterEntityBattle GetEntityByGuid(string characterGuid, BattleType battleType) => playersComponent.GetEntityByGuid(characterGuid, battleType);
    //sideComponent
    public TeamSide TeamSide => sideComponent.TeamSide;
    public Variant Variant => sideComponent.Variant;
    public void SetSide(TeamSide teamSide) => sideComponent.SetSide(teamSide);
    public void SetVariant(Variant variant) => sideComponent.SetVariant(variant);
    public void ResetSideAndVariant() => sideComponent.ResetSideAndVariant();
    //customLoadoutComponent
    public bool IsCustomLoadout => customLoadoutComponent.IsCustomLoadout;
    public string CustomName => customLoadoutComponent.CustomName;
    public void SetCustomName(string name) => customLoadoutComponent.SetCustomName(name);
    //persistenceComponent
    public void Import(TeamSaveData teamSaveData) => persistenceComponent.Import(teamSaveData);
    public TeamSaveData Export() => persistenceComponent.Export();
    #endregion    
}
