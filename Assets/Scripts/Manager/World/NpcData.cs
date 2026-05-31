using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

[CreateAssetMenu(fileName = "NpcData", menuName = "ScriptableObject/World/NpcData")]
public class NpcData : ScriptableObject
{
    public string NpcId;
    public Gender Gender;
}
