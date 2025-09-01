using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentRestrictionLearn
{
    public List<Element> AllowedElements { get; private set; }
    public List<Position> AllowedPositions { get; private set; }
    public List<Gender> AllowedGenders { get; private set; }
    public List<CharacterSize> AllowedSizes { get; private set; }

    public MoveComponentRestrictionLearn(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        AllowedElements = moveData.AllowedElements;
        AllowedPositions = moveData.AllowedPositions;
        AllowedGenders = moveData.AllowedGenders;
        AllowedSizes = moveData.AllowedSizes;
    }
}
