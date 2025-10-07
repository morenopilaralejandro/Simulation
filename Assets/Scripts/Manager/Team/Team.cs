using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Localization;

public class Team
{
    #region Components
    [SerializeField] private TeamComponentAttributes attributesComponent;
    [SerializeField] private LocalizationComponentString localizationStringComponent;
    [SerializeField] private TeamComponentFormation formationComponent;
    [SerializeField] private TeamComponentKit kitComponent;
    [SerializeField] private TeamComponentLevels levelsComponent;
    [SerializeField] private TeamComponentPlayers playersComponent;
    [SerializeField] private TeamComponentSide sideComponent;
    #endregion

    #region Initialize
    public void Initialize(TeamData teamData)
    {
        BindComponents();

        attributesComponent.Initialize(teamData);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Team,
            teamData.TeamId,
            new [] { LocalizationField.Name }
        );

        formationComponent.Initialize(teamData);
        kitComponent.Initialize(teamData);
        levelsComponent.Initialize(teamData);
        playersComponent.Initialize(teamData);
    }

    private void BindComponents()
    {
        formationComponent.SetTeam(this);
        kitComponent.SetTeam(this);
        levelsComponent.SetTeam(this);
        sideComponent.SetTeam(this);
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
    #endregion    
}
