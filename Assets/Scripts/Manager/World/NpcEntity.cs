using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntity : MonoBehaviour
{
    #region Fields

    private Npc npc;

    #endregion

    #region Components

    // [SerializeField] private IInteractable interactableComponent;

    #endregion

    #region Initialize

    /*

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
        //appearanceComponent.Initialize(character.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);

        //Kit kit = KitManager.Instance.GetKit(kitData.KitId);
        //character.ApplyKit(kit, Variant.Home, role);
    }

    public void Initialize(NpcData npcData)
    {
        npc = new Npc(npcData);
        //appearanceComponent.Initialize(npc.AppearanceComponent);

        if (interactableComponent != null)
            interactableComponent.Initialize(this);

        //appearanceComponent.ApplyClothes(npcData);
    }

    */

    #endregion

    #region API

    public void SetNpc(Npc npc) => this.npc = npc;
    public Npc Npc => npc;
   
    #endregion

}
