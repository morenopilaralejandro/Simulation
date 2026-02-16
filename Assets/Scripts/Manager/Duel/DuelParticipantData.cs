using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class DuelParticipantData
{
    public CharacterEntityBattle CharacterEntityBattle;
    public Category? Category;
    public DuelAction? Action;
    public DuelCommand? Command;
    public Move Move;

    public bool IsDirect;

    public bool IsComplete =>
        CharacterEntityBattle != null &&
        Category.HasValue &&
        Action.HasValue &&
        Command.HasValue;
}
