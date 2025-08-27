using UnityEngine;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "FormationData", menuName = "ScriptableObject/FormationData")]
public class FormationData : ScriptableObject
{
    public string FormationId;
    public string[] CoordIds = new string[11];
    public Position[] Positions = new Position[11];
    public int Kickoff0;
    public int Kickoff1;
}
