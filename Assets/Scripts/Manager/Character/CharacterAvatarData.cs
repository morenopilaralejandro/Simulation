using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterAvatarData
{
    public string AvatarName { get; private set; }
    public CharacterSize CharacterSize { get; private set; }
    public Gender Gender { get; private set; }
    public Element Element { get; private set; }
    public Position Position { get; private set; }

    public HairStyle HairStyle { get; private set; }
    public HairColorType HairColorType { get; private set; }
    public EyeColorType EyeColorType { get; private set; }
    public BodyColorType BodyColorType { get; private set; }
    public PortraitSize PortraitSize { get; private set; }
}
