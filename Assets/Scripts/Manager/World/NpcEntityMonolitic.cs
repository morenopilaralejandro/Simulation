using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class NpcEntityMonolitic : NpcEntity
{
    #region Fields
    [SerializeField] private NpcType npcType = NpcType.Monolitic;
    [SerializeField] private NpcData npcData;

    // npc appearance private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Components

    // [SerializeField] private CharacterComponentAppearanceBattle appearanceComponentBattle;

    #endregion

    #region Initialize

    public void Start() 
    {
        Initialize(npcData);
    }

    public void Initialize(NpcData npcData)
    {
        base.SetNpc(new Npc(npcData, null, npcType));

        /*
        appearanceComponent.Initialize(characterData, null, null);
        appearanceComponent.SetKit(KitManager.Instance.GetKit(kitData.KitId), variant, role);
        _ = appearanceComponentBattle.LoadKitAsync();
        */
    }

    #endregion

    #region API

    //public Sprite PortraitSize => appearanceComponent.PortraitSize;

    #endregion

}
