using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntityCharacter : NpcEntity
{
    #region Fields
    [SerializeField] private NpcType npcType = NpcType.Character;
    [SerializeField] private CharacterData characterData;
    [SerializeField] private KitData kitData;
    [SerializeField] private Variant variant;
    [SerializeField] private Role role;

    private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Components

    [SerializeField] private CharacterComponentAppearanceBattle appearanceComponentBattle;

    #endregion

    #region Initialize

    public void Start() 
    {
        Initialize(characterData);
    }

    public void Initialize(CharacterData characterData)
    {
        base.SetNpc(new Npc(null, characterData, npcType));

        appearanceComponent = new CharacterComponentAppearance(characterData, null, null);
        appearanceComponentBattle.Initialize(appearanceComponent);
        appearanceComponent.SetKit(KitManager.Instance.GetKit(kitData.KitId), variant, role);
        _ = appearanceComponentBattle.LoadKitAsync();
    }

    #endregion

    #region API

    public PortraitSize PortraitSize => appearanceComponent.PortraitSize;

    #endregion

}
