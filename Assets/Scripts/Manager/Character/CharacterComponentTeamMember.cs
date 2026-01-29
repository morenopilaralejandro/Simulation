using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    private Character character;

    [SerializeField] private TeamSide teamSide;
    [SerializeField] private FormationCoord formationCoord;

    public TeamSide TeamSide => teamSide;
    public FormationCoord FormationCoord => formationCoord;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
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
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character != character) return;

        this.teamSide = team.TeamSide;   
        this.formationCoord = new FormationCoord(formationCoord);

        if (!character.IsOnUsersTeam())
            this.formationCoord.FlipDefaultPosition();

        LogManager.Trace($"[CharacterComponentTeamMember] {this.character.CharacterId} assigned to team {team.TeamId} on side {team.TeamSide} at {formationCoord.FormationCoordId}", this);
    }

    public bool IsOnUsersTeam() => this.teamSide == BattleTeamManager.Instance.GetUserSide();
    public bool IsSameTeam(Character otherCharacter) => this.teamSide == otherCharacter.TeamSide;
    public TeamSide GetOpponentSide() => this.character.TeamSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
    public Team GetTeam() => BattleTeamManager.Instance.Teams[this.teamSide];
    public Team GetOpponentTeam() => BattleTeamManager.Instance.Teams[GetOpponentSide()];
    public List<Character> GetTeammates() => GetTeam().CharacterList;
    public List<Character> GetOpponents() => GetOpponentTeam().CharacterList;

}
