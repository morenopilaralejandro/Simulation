using UnityEngine;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObject/TeamData")]
public class TeamData : ScriptableObject
{
    public string TeamId;
    public string FormationId;
    public string KitId;
    public int Lv;
    public string[] CharacterIds = new string[16];
}
