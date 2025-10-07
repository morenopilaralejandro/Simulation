using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    private Character character;

    [SerializeField] private TeamSide teamSide;
    [SerializeField] private FormationCoord formationCoord;

    public TeamSide TeamSide => teamSide;
    public FormationCoord FormationCoord => formationCoord;

    public void SetCharacter(Character character)
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
        if (character == this.character)
        {
            this.teamSide = team.TeamSide;
            
            if (teamSide != BattleManager.Instance.GetUserSide())
                formationCoord.FlipDefaultPosition();

            this.formationCoord = formationCoord;

            LogManager.Trace($"[CharacterComponentTeamMember] {this.character.CharacterId} assigned to team {team.TeamId} on side {team.TeamSide} at {formationCoord.FormationCoordId}", this);
        }
    }
}
