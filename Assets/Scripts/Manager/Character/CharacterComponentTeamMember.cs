using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private TeamSide teamSide;
    [SerializeField] private FormationCoord formationCoord;
    private BattleManager battleManager;

    public TeamSide TeamSide => teamSide;
    public FormationCoord FormationCoord => formationCoord;

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
    }

    private void HandleAssignCharacterToTeamBattle(
        CharacterEntityBattle characterEntityBattle, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.characterEntityBattle != characterEntityBattle) return;

        this.teamSide = team.TeamSide;   
        this.formationCoord = new FormationCoord(formationCoord);
        this.battleManager = BattleManager.Instance;

        if (!characterEntityBattle.IsOnUsersTeam()) 
        {
            this.formationCoord.FlipDefaultPosition();    
            characterEntityBattle.SetFormationDirection(Vector2.down);
        } else 
        {
            characterEntityBattle.SetFormationDirection(Vector2.up);
        }

        this.characterEntityBattle.ForceEquipWing(WingFactory.CreateFromData("wing-00001-angel")); //TODO use character data

        this.characterEntityBattle.SetKit(
            team,
            formationCoord.Position
        );

        LogManager.Trace($"[CharacterComponentTeamMember] {this.characterEntityBattle.CharacterId} assigned to team {team.TeamId} on side {team.TeamSide} at {formationCoord.FormationCoordId}", this);
    }

    public bool IsOnUsersTeam() => teamSide == battleManager.GetUserSide();
    public bool IsSameTeam(CharacterEntityBattle otherCharacter) => teamSide == otherCharacter.TeamSide;
    public TeamSide GetOpponentSide() => teamSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
    public Team GetTeam() => battleManager.Teams[teamSide];
    public Team GetOpponentTeam() => battleManager.Teams[GetOpponentSide()];
    public List<CharacterEntityBattle> GetTeammates() => GetTeam().GetCharacterEntities(battleManager.CurrentType);
    public List<CharacterEntityBattle> GetOpponents() => GetOpponentTeam().GetCharacterEntities(battleManager.CurrentType);

}
