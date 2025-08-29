using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentAttribute : MonoBehaviour
{
    [SerializeField] private string characterId;
    [SerializeField] private CharacterSize characterSize;
    [SerializeField] private Gender gender;
    [SerializeField] private Element element;
    [SerializeField] private Position position;
    [SerializeField] private ControlType controlType;

    public string GetCharacterId() => characterId;
    public CharacterSize GetCharacterSize() => characterSize;
    public Gender GetGender() => gender;
    public Element GetElement() => element;
    public Position GetPosition() => position;
    public ControlType GetControlType() => controlType;

    public void Initialize(CharacterData characterData)
    {
        characterId = characterData.CharacterId;
        characterSize = characterData.CharacterSize;
        gender = characterData.Gender;
        element = characterData.Element;
        position = characterData.Position;
    }
}
