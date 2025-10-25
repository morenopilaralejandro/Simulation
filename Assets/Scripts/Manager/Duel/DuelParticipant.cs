using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class DuelParticipant
{
    #region Shared
    public Character Character { get; }
    public Category Category { get; }
    public DuelAction Action { get; }
    public DuelCommand Command { get; }
    public Move Move { get; }
    public float Damage { get; set; }
    public Element CurrentElement { get; set; }
    #endregion

    #region Field
    public bool IsKeeperDuel { get; }
    #endregion

    #region Shoot
    public bool IsDirect { get; }
    #endregion

    public DuelParticipant(
        Character character,
        Category category,
        DuelAction action,
        DuelCommand command,
        Move move,
        bool isKeeperDuel,
        bool isDirect)
    {
        Character = character;
        Category = category;
        Action = DuelManager.Instance.GetActionByCategory(category);
        Command = command;
        Move = move;
        IsDirect = isDirect;
        IsKeeperDuel = isKeeperDuel;

        CurrentElement = move == null ? character.Element : Move.Element;
        Damage = DamageCalculator.GetDamage(
            Category, 
            Command, 
            character, 
            Move,
            IsDirect,
            IsKeeperDuel);        
    }
}
