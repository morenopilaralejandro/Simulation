using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

[CreateAssetMenu(fileName = "MoveData", menuName = "ScriptableObject/MoveData")]
public class MoveData : ScriptableObject
{
    public string MoveId;
    public Category Category;
    public Element Element;
    public Trait Trait;
    public GrowthType GrowthType;
    public GrowthRate GrowthRate;

    public int Cost;
    public int BasePower;
    public int StunDamage;
    public int AuraDamage;
    public int Difficulty;
    public int FaultRate;
    public int Participants;

    public List<Element> AllowedElements = new List<Element>();
    public List<Position> AllowedPositions = new List<Position>();
    public List<Gender> AllowedGenders = new List<Gender>();
    public List<CharacterSize> AllowedSizes = new List<CharacterSize>();

    public List<Element> RequiredParticipantElements = new List<Element>();
    public List<string> RequiredParticipantMoves = new List<string>();
}
