using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

public class NpcComponentCharacter : MonoBehaviour
{
    private NpcEntity npcEntity;
    private Character character;

    [SerializeField] private string characterId;
    [SerializeField] private bool useCharacterData;

    public bool UseCharacterData => useCharacterData;
    public Character Character => character;

    public void Initialize(NpcEntity npcEntity)
    {
        this.npcEntity = npcEntity;
        this.character = null;
        if (string.IsNullOrEmpty(characterId)) return;
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
        this.character = new Character(characterData);
    }

}
