using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentAttributes : MonoBehaviour
{
    [SerializeField] private string characterId;
    [SerializeField] private CharacterSize characterSize;
    [SerializeField] private Gender gender;
    [SerializeField] private Element element;
    [SerializeField] private Position position;

    public string CharacterId => characterId;
    public CharacterSize CharacterSize => characterSize;
    public Gender Gender => gender;
    public Element Element => element;
    public Position Position => position;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.characterId = characterData.CharacterId;
        this.characterSize = characterData.CharacterSize;
        this.gender = characterData.Gender;
        this.element = characterData.Element;
        this.position = characterData.Position;
    }
}
