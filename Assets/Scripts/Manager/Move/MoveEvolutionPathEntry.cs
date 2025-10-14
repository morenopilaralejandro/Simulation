using UnityEngine;
using System;
using Simulation.Enums.Move;

[Serializable]
public class MoveEvolutionPathEntry
{
    public MoveEvolution previous;
    public MoveEvolution next;
}
