using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

[CreateAssetMenu(fileName = "FormationData", menuName = "ScriptableObject/FormationData")]
public class FormationData : ScriptableObject
{
    public string FormationId;
    public BattleType BattleType;
    public List<string> CoordIds = new List<string>();
    public List<Position> Positions = new List<Position>();
    public int Kickoff0;
    public int Kickoff1;
}
