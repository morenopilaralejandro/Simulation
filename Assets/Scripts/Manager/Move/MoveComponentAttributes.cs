using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentAttributes
{
    private Move move;

    public string MoveId { get; private set; }
    public Category Category { get; private set; }
    public Element Element { get; private set; }
    public Trait Trait { get; private set; }

    public int Cost { get; private set; }
    public int BasePower { get; private set; }
    public int StunDamage { get; private set; }
    public int AuraDamage { get; private set; }
    public int Difficulty { get; private set; }
    public int FaultRate { get; private set; }

    public MoveComponentAttributes(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
        this.MoveId = moveData.MoveId;
        this.Category = moveData.Category;
        this.Element = moveData.Element;
        this.Trait = moveData.Trait;
        
        this.Cost = moveData.Cost;
        this.BasePower = moveData.BasePower;
        this.StunDamage = moveData.StunDamage;
        this.AuraDamage = moveData.AuraDamage;
        this.Difficulty = moveData.Difficulty;
        this.FaultRate = moveData.FaultRate;
    }
}
