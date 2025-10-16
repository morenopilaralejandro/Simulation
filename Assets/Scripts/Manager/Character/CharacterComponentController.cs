using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class CharacterComponentController : MonoBehaviour
{
    private Character character;

    [SerializeField] private bool isControlled => CharacterChangeControlManager.Instance.CurrentCharacter == this.character;

    public bool IsControlled => isControlled;

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

    void Update()
    {
        if (!this.isControlled || !this.character.CanMove()) return;

        Vector2 moveInput = InputManager.Instance.GetMove();
        float speed = this.character.GetMovementSpeed();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * speed * Time.deltaTime;
        if (moveInput.sqrMagnitude > 0.01f)
            LogManager.Trace($"[CharacterComponentController] " +
                $"Character: {character.CharacterId}, " +
                $"Input: {moveInput}, " +
                $"Speed: {speed}, " +
                $"Position: {transform.position}");
        transform.Translate(move, Space.World);
    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character && team.TeamSide != BattleTeamManager.Instance.GetUserSide())
        {
            this.enabled = false;
        }
    }

}
