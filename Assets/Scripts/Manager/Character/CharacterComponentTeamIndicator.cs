using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentTeamIndicator : MonoBehaviour
{
    private Character character;

    [SerializeField] private SpriteRenderer indicatorRenderer;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;
        CharacterEvents.OnControlChange += HandleOnControlChange;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
        CharacterEvents.OnControlChange -= HandleOnControlChange;
    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character)
        {
            ChangeColor(team.TeamSide, false);
        }
    }

    private void HandleOnControlChange(Character character, TeamSide teamSide)
    {
        if (teamSide != this.character.TeamSide) return;

        if (this.character == character)
        {
            ChangeColor(this.character.TeamSide, true);
        } else {
            ChangeColor(this.character.TeamSide, false);
        }
    }

    private void ChangeColor(TeamSide teamSide, bool highlight) 
    {
        indicatorRenderer.color = ColorManager.GetTeamIndicatorColor(teamSide, highlight);
    }

}
