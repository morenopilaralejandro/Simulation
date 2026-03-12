using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentAttributes
{
    public string CharacterId { get; private set; }
    public string CharacterGuid { get; private set; }
    public CharacterSize CharacterSize { get; private set; }
    public Gender Gender { get; private set; }
    public Element Element { get; private set; }
    public Position Position { get; private set; }

    public CharacterComponentAttributes(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        CharacterId = characterData.CharacterId;
        CharacterSize = characterData.CharacterSize;
        Gender = characterData.Gender;
        Element = characterData.Element;
        Position = characterData.Position;

        if (characterSaveData != null)
            CharacterGuid = characterSaveData.CharacterGuid;
    }
}
