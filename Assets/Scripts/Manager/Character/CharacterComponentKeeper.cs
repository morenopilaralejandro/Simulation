using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentKeeper : MonoBehaviour
{
    private Character character;

    [SerializeField] private Collider keeperCollider;   //inspector

    [SerializeField] private bool isKeeper;

    public bool IsKeeper => isKeeper;

    public void UpdateKeeperColliderState()
    {
        if (keeperCollider != null)
            keeperCollider.enabled = isKeeper;
    }

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
            this.isKeeper = formationCoord.Position == Position.GK ? true : false;
            UpdateKeeperColliderState();
        }
    }
}
