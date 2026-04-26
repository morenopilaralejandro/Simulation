using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;

public class DuelParticipant
{
    #region Shared
    public CharacterEntityBattle CharacterEntityBattle { get; }
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
        CharacterEntityBattle characterEntityBattle,
        Category category,
        DuelAction action,
        DuelCommand command,
        Move move,
        bool isKeeperDuel,
        bool isDirect)
    {
        CharacterEntityBattle = characterEntityBattle;
        Category = category;
        Action = DuelManager.Instance.GetActionByCategory(category);
        Command = command;
        Move = move;
        IsDirect = isDirect;
        IsKeeperDuel = isKeeperDuel;

        CurrentElement = move == null ? characterEntityBattle.Element : Move.Element;
        Damage = DamageCalculator.GetDamage(
            Category, 
            Command, 
            characterEntityBattle, 
            Move,
            IsDirect,
            IsKeeperDuel);        
    }
}
