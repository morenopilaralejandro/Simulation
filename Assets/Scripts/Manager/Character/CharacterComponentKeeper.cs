using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentKeeper : MonoBehaviour
{
    [SerializeField] private CharacterComponentAttribute attributeComponent;
    [SerializeField] private bool isKeeper;
    [SerializeField] private Collider keeperCollider;

    public bool IsKeeper() => isKeeper;

    public void UpdateKeeperColliderState()
    {
        if (keeperCollider != null)
            keeperCollider.enabled = isKeeper;
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
        int teamIndex, 
        FormationCoord formationCoord, 
        ControlType controlType)
    {
        if (character.GetCharacterId() == attributeComponent.GetCharacterId())
        {
            this.isKeeper = formationCoord.Position == Position.GK ? true : false;
            UpdateKeeperColliderState();
        }
    }
}
