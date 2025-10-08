using UnityEngine;
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
        if (this.character == character)
        {
            this.teamSide = team.TeamSide;
            
            if (!character.IsOnUsersTeam())
                formationCoord.FlipDefaultPosition();

            this.formationCoord = formationCoord;

            LogManager.Trace($"[CharacterComponentTeamMember] {this.character.CharacterId} assigned to team {team.TeamId} on side {team.TeamSide} at {formationCoord.FormationCoordId}", this);
        }
    }

    public bool IsOnUsersTeam() 
    {
        return this.teamSide == BattleTeamManager.Instance.GetUserSide();
    }

    public bool IsSameTeam(Character otherCharacter) 
    {
        return this.teamSide == otherCharacter.TeamSide;
    }

    public TeamSide GetOpponentSide() 
    {
        return this.character.TeamSide == TeamSide.Home ? TeamSide.Away : TeamSide.Home;
    }
}
