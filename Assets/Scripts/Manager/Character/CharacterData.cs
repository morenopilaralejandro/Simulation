using UnityEngine;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string CharacterId;
    public PortraitSize PortraitSize;
    public CharacterSize CharacterSize;
    public Gender Gender;
    public Element Element;
    public Position Position;
    public int Hp; 
    public int Sp;
    public int Kick;
    public int Body;
    public int Control;
    public int Guard;
    public int Speed;
    public int Stamina;
    public int Technique;
    public int Luck;
    public int Courage;
    public int Freedom;
    public string[] MoveIds = new string[4];
    public int[] MoveLvs = new int[4];
}

