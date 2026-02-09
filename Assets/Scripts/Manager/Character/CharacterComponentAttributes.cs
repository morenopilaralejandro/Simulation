using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentAttributes : MonoBehaviour
{
    [SerializeField] private string characterId;
    [SerializeField] private string characterGuid;
    [SerializeField] private CharacterSize characterSize;
    [SerializeField] private Gender gender;
    [SerializeField] private Element element;
    [SerializeField] private Position position;

    public string CharacterId => characterId;
    public string CharacterGuid => characterGuid;
    public CharacterSize CharacterSize => characterSize;
    public Gender Gender => gender;
    public Element Element => element;
    public Position Position => position;

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        characterId = characterData.CharacterId;
        characterSize = characterData.CharacterSize;
        gender = characterData.Gender;
        element = characterData.Element;
        position = characterData.Position;

        if (characterSaveData != null)
            characterGuid = characterSaveData.CharacterGuid;
    }
}
