using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentEvolution
{
    private Move move;
    private MoveEvolution currentEvolution;

    public GrowthType GrowthType { get; private set; }
    public GrowthRate GrowthRate { get; private set; }
    public int TimesUsedTotal { get; private set; }
    public int TimesUsedCurrentEvolution { get; private set; }

    public MoveComponentEvolution(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
        this.currentEvolution = MoveEvolution.None;
        GrowthType = moveData.GrowthType;
        GrowthRate = moveData.GrowthRate;

    }

    /*
    dictionaries
    evolutionpath GrowthType, MoveEvolutionPrevios, MoveEvolutionNext 
    Delchi, None, Delta
    Delchi, Delta, Chi
    Delchi, Chi, Phi
    S, None, S
    S, S, SS
    S, SS, SS
    Uraga, None, Ura
    Uraga, Ura, Galaxy
    Uraga, Galaxy, Legend

    evolutionExtraPower GrowthRate, MoveEvolution, int extrapower
    Fast, Delta, 4
    Fast, Chi, 8
    Fast, Phi, 16
    Medium, Delta, 16
    Medium, Chi, 32
    Medium, Phi, 64
    Slow, Delta, 64
    Slow, Chi, 96
    Slow, Phi, 128

    Fast, S, 2
    Fast, SS, 4
    Fast, SSS, 6
    Medium, S, 6
    Medium, SS, 8
    Medium, SSS, 10
    Slow, S, 10
    Slow, SS, 12
    Slow, SSS, 14

    Fast, Ura, 4
    Fast, Galaxy, 8 
    Fast, Legend, 12
    Medium, Ura, 12
    Medium, Galaxy, 16
    Medium, Legend, 20
    Slow, Ura, 20
    Slow, Galaxy, 24 
    Slow, Legend, 28

    evolutionUsageThresshold GrowthType, GrowthRate, MoveEvolution, int usageThresshold
    Delchi, Fast, None, 8
    Delchi, Fast, Delta, 16

    Delchi, Medium, None, 16
    Delchi, Medium, Delta, 24

    Delchi, Slow, None, 24
    Delchi, Slow, Delta, 32

    S, Fast, None, 2
    S, Fast, Delta, 4

    S, Medium, None, 4
    S, Medium, Delta, 6

    S, Slow, None, 6
    S, Slow, Delta, 8

    Uraga, Fast, None, 4
    Uraga, Fast, Delta, 8

    Uraga, Medium, None, 6
    Uraga, Medium, Delta, 10

    Uraga, Slow, None, 8
    Uraga, Slow, Delta, 12

    Methods
    IsLastEvolution: no next evo in evolutionpath
    Evolve: currentEvolution = next evo in evolutionpath
    GetExtraPower: if none then 0 else look evolutionExtraPower
    GetUsageThresshold: look evolutionUsageThresshold
    ProgressEvolution: TimesUsed++ and if thresshold is reached call method evolve. However, the last evolution can not be reached by this mean.
    LimitBreak: this method is called to evolve to the last evolution. For example from SS to SSS. But it only works is you are in the previos evolution to the last one.
    
    */
}
