using UnityEngine;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "NpcData", menuName = "ScriptableObject/World/NpcData")]
public class NpcData : ScriptableObject
{
    public string NpcId;
    public BodyColorType BodyColorType;
    public EyeColorType EyeColorType;
    public HairColorType HairColorType;
    public HairStyle HairStyle;
    public PortraitSize PortraitSize;
    public Gender Gender;
}
