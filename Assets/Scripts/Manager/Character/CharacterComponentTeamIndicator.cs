using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentTeamIndicator : MonoBehaviour
{
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private SpriteRenderer indicatorRenderer;

    public void Initialize(CharacterEntityBattle characterEntityBattle) 
    {
        this.characterEntityBattle = characterEntityBattle;
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
        CharacterEntityBattle character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.characterEntityBattle == character)
        {
            ChangeColor(team.TeamSide, false);
        }
    }

    private void HandleOnControlChange(CharacterEntityBattle character, TeamSide teamSide)
    {
        if (teamSide != this.characterEntityBattle.TeamSide) return;

        if (this.characterEntityBattle == character && !characterEntityBattle.IsEnemyAI)
        {
            ChangeColor(this.characterEntityBattle.TeamSide, true);
        } else {
            ChangeColor(this.characterEntityBattle.TeamSide, false);
        }
    }

    private void ChangeColor(TeamSide teamSide, bool highlight) 
    {
        indicatorRenderer.color = ColorManager.GetTeamIndicatorColor(teamSide, highlight);
    }

}
