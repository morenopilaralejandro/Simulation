using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

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
    public KitColor KitBaseColor;
    public KitColor KitDetailColor;
    public Sprite PortraitSprite;
}
