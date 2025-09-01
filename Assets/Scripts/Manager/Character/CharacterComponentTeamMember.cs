using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private int teamIndex;
    [SerializeField] private FormationCoord formationCoord;
    [SerializeField] private ControlType controlType;

    public int GetTeamIndex() => teamIndex;
    public FormationCoord GetFormationCoord() => formationCoord;
    public ControlType GetControlType() => controlType;

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
        int teamIndex, 
        FormationCoord formationCoord, 
        ControlType controlType)
    {
        if (character.CharacterId() == attributeComponent.GetCharacterId())
        {
            this.teamIndex = teamIndex;
            this.controlType = controlType;
            
            if (teamIndex != BattleManager.Instance.GetLocalTeamIndex())
                formationCoord.FlipDefaultPosition();

            this.formationCoord = formationCoord;

            LogManager.Trace($"[CharacterComponentTeamMember] {attributeComponent.GetCharacterId()} assigned to team {teamIndex} at {formationCoord.FormationCoordId}", this);
        }
    }
}
