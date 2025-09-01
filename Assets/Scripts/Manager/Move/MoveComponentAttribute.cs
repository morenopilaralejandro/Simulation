using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentAttribute
{
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

    public MoveComponentAttribute(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        MoveId = moveData.MoveId;
        Category = moveData.Category;
        Element = moveData.Element;
        Trait = moveData.Trait;
        
        Cost = moveData.Cost;
        BasePower = moveData.BasePower;
        StunDamage = moveData.StunDamage;
        AuraDamage = moveData.AuraDamage;
        Difficulty = moveData.Difficulty;
        FaultRate = moveData.FaultRate;
    }
}
