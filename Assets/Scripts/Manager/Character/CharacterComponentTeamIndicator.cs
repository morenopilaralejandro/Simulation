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
        ChangeColor(false);
    }

    private void OnEnable()
    {
        CharacterEvents.OnControlChange += HandleOnControlChange;    
    }

    private void OnDisable()
    {
        CharacterEvents.OnControlChange -= HandleOnControlChange;
    }

    private void HandleOnControlChange(Character character, TeamSide teamSide)
    {
        if (this.character == character)
        {
            ChangeColor(true);
        } else {
            ChangeColor(false);
        }
    }

    private void ChangeColor(bool highlight) 
    {
        indicatorRenderer.color = ColorManager.GetTeamIndicatorColor(this.character.TeamSide, highlight);
    }

}
