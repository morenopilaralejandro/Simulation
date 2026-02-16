using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObject/TeamData")]
public class TeamData : ScriptableObject
{
    public string TeamId;
    public string KitId;
    public int Lv;

    public string FullBattleFormationId;
    public List<string> FullBattleCharacterIds = new List<string>();

    public string MiniBattleFormationId;
    public List<string> MiniBattleCharacterIds = new List<string>();
}
