using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObject/TeamData")]
public class TeamData : ScriptableObject
{
    public string TeamId;
    public string FormationId;
    public string KitId;
    public int Lv;
    public List<string> CharacterIds = new List<string>();
}
