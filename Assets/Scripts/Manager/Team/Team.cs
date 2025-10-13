using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Localization;

public class Team
{
    #region Components
    private TeamComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private TeamComponentFormation formationComponent;
    private TeamComponentKit kitComponent;
    private TeamComponentLevels levelsComponent;
    private TeamComponentPlayers playersComponent;
    private TeamComponentSide sideComponent;
    #endregion

    #region Initialize
    public Team(TeamData teamData) 
    {
        Initialize(teamData);
    }

    public void Initialize(TeamData teamData)
    {
        attributesComponent = new TeamComponentAttributes(teamData, this);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Team,
            teamData.TeamId,
            new [] { LocalizationField.Name }
        );

        formationComponent = new TeamComponentFormation(teamData, this);
        kitComponent = new TeamComponentKit(teamData, this);
        levelsComponent = new TeamComponentLevels(teamData, this);
        playersComponent = new TeamComponentPlayers(teamData, this);
        sideComponent = new TeamComponentSide(teamData, this);
    }

    public void Deinitialize()
    {
        sideComponent.Deinitialize();
    }
    #endregion

    #region API
    //attributeComponent
    public string TeamId => attributesComponent.TeamId;
    //localizationComponent
    public string TeamName => localizationStringComponent.GetString(LocalizationField.Name);
    //formationComponent
    public Formation Formation => formationComponent.Formation;    
    public void SetFormation(Formation formation) => formationComponent.SetFormation(formation);
    //kitComponent
    public Kit Kit => kitComponent.Kit;    
    public void SetKit(Kit kit) => kitComponent.SetKit(kit);
    //levelsComponent
    public int Level => levelsComponent.Level;
    //playersComponent
    public List<CharacterData> CharacterDataList => playersComponent.CharacterDataList;
    public List<Character> CharacterList => playersComponent.CharacterList;
    //sideComponent
    public TeamSide TeamSide => sideComponent.TeamSide;
    public Variant Variant => sideComponent.Variant;
    #endregion    
}
