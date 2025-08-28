using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "FormationData", menuName = "ScriptableObject/FormationData")]
public class FormationData : ScriptableObject
{
    public string FormationId;
    public List<string> CoordIds = new List<string>();
    public List<Position> Positions = new List<Position>();
    public int Kickoff0;
    public int Kickoff1;
}
