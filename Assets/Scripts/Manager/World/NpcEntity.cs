using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

public class NpcEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private bool isGeneric;
    [SerializeField] private NpcData npcData;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private KitData kitData;
    [SerializeField] private Role role;
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
            Initialize(npcData);
        else 
            Initialize(characterData);
    }

    public void Initialize(CharacterData characterData)
    {
        character = new Character(characterData);
        appearanceComponent.Initialize(character.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);

        Kit kit = KitManager.Instance.GetKit(kitData.KitId);
        character.ApplyKit(kit, Variant.Home, role);
    }

    public void Initialize(NpcData npcData)
    {
        npc = new Npc(npcData);
        appearanceComponent.Initialize(npc.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);

        appearanceComponent.ApplyClothes(npcData);
    }

    #endregion

    #region API

    public Character Character => character;
    public Npc Npc => npc;
    public bool IsGeneric => isGeneric;

    public string NpcName => isGeneric ? npc.NpcName : character.CharacterNick;
    public Sprite PortraitSprite => isGeneric ? npcData.PortraitSprite : character.PortraitSprite;
    public PortraitSize PortraitSize => isGeneric ? npc.PortraitSize : character.PortraitSize;

    #endregion

    #region API Npc

    public string NpcId => isGeneric ? npc.NpcId : null;

    #endregion
}
