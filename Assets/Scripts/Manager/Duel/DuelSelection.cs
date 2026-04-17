using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;

public class DuelSelection
{
    public int ParticipantIndex;
    public CharacterEntityBattle CharacterEntityBattle;
    public Category Category;

    public DuelCommand Command;
    public Move Move;
    public bool IsReady;
}
