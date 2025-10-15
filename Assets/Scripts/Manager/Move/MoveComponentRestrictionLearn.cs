using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentRestrictionLearn
{
    private Move move;

    public List<Element> AllowedElements { get; private set; }
    public List<Position> AllowedPositions { get; private set; }
    public List<Gender> AllowedGenders { get; private set; }
    public List<CharacterSize> AllowedSizes { get; private set; }

    public MoveComponentRestrictionLearn(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
        this.AllowedElements = moveData.AllowedElements;
        this.AllowedPositions = moveData.AllowedPositions;
        this.AllowedGenders = moveData.AllowedGenders;
        this.AllowedSizes = moveData.AllowedSizes;
    }
}
