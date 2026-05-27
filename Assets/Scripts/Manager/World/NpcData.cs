using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

[CreateAssetMenu(fileName = "NpcData", menuName = "ScriptableObject/World/NpcData")]
public class NpcData : ScriptableObject
{
    public string NpcId;
    public Gender Gender;

    // for basic body use NpcId
    public bool IsLayered;
    public BodyColorType BodyColorType;
    public EyeColorType EyeColorType;
    public HairColorType HairColorType;
    public HairStyle HairStyle;
    public PortraitSize PortraitSize;
    public string KitId;
    public Variant Variant;
    public Role Role;
}
