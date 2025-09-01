using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentEvolution
{
    public GrowthType GrowthType { get; private set; }
    public GrowthRate GrowthRate { get; private set; }

    public MoveComponentEvolution(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        GrowthType = moveData.GrowthType;
        GrowthRate = moveData.GrowthRate;
    }
}
