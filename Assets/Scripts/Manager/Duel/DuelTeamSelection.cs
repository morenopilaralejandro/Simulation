using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class DuelSelection
{
    public int ParticipantIndex;
    public Character Character;
    public Category Category;

    public DuelCommand Command;
    public Move Move;
    public bool IsReady;
}
