using UnityEngine;
using Simulation.Enums.Character;

public class NpcEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private NpcData npcData;
    [SerializeField] private string characterId;
    [SerializeField] private bool isGeneric;
    private Character character;
    private Npc npc;
    #endregion

    #region Components

    [SerializeField] private CharacterComponentAppearanceWorld appearanceComponent;
    [SerializeField] private NpcComponentInteractable interactableComponent;

    #endregion

    #region Initialize

    public void Start() 
    {
        if (isGeneric) 
        {
            Initialize(npcData);
        } else 
        {
            CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterId);
            Initialize(characterData);
        }
    }

    public void Initialize(CharacterData characterData)
    {
        character = new Character(characterData);
        appearanceComponent.Initialize(character.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);
    }

    public void Initialize(NpcData npcData)
    {
        npc = new Npc(npcData);
        appearanceComponent.Initialize(npc.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);
    }

    #endregion

    #region API
    public Character Character => character;
    public Npc Npc => npc;
    public bool IsGeneric => isGeneric;

    public string NpcName => isGeneric ? npc.NpcName : character.CharacterNick;
    public Sprite PortraitSprite => isGeneric ? npc.PortraitSprite : character.PortraitSprite;
    public PortraitSize PortraitSize => isGeneric ? npc.PortraitSize : character.PortraitSize;
    #endregion

    #region API Npc
    public string NpcId => isGeneric ? npc.NpcId : null;
    #endregion
}
