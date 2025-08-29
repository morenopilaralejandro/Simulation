using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private int teamIndex;
    [SerializeField] private FormationCoord formationCoord;

    public int GetTeamIndex() => teamIndex;
    public FormationCoord GetFormationCoord() => formationCoord;

    private void OnEnable()
    {
        TeamEvents.OnAssignToTeam += HandleAssign;    
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignToTeam -= HandleAssign;
    }

    private void HandleAssign(Character character, Team team, int teamIndex, FormationCoord formationCoord)
    {
        if (character.GetCharacterId() == attributeComponent.GetCharacterId())
        {
            this.teamIndex = teamIndex;
            this.formationCoord = formationCoord;
            LogManager.Trace($"[CharacterComponentTeamMember] {attributeComponent.GetCharacterId()} assigned to team {teamIndex} at {formationCoord.FormationCoordId}", this);
        }
    }
}
