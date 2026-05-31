using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.World;

public class Npc
{
    #region Components

    private NpcComponentAttributes attributesComponent;
    //private LocalizationComponentString localizationStringComponent;
    //private CharacterComponentAppearance appearanceComponent;

    #endregion

    #region Initialize

    public Npc(NpcData npcData, CharacterData characterData, NpcType npcType) 
    {
        Initialize(npcData, characterData, npcType);
    }

    public void Initialize(NpcData npcData, CharacterData characterData, NpcType npcType)
    {
        attributesComponent = new NpcComponentAttributes(npcData, characterData, npcType);

        /*
        switch (NpcType)
        {
            case NpcType.Character:
                localizationStringComponent = new LocalizationComponentString(
                    LocalizationEntity.Character,
                    NpcId,
                    new[] { LocalizationField.Nick }
                );
                break;
            default:
                localizationStringComponent = new LocalizationComponentString(
                    LocalizationEntity.Npc,
                    NpcId,
                    new[] { LocalizationField.Name }
                );
                break;
        }
        */
    }

    #endregion

    #region API

    // attributesComponent
    public string NpcId => attributesComponent.NpcId;
    public Gender Gender => attributesComponent.Gender;
    public NpcType NpcType => attributesComponent.NpcType;
    // localizationComponent
    // public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    // public string NpcName => localizationStringComponent?.GetString(LocalizationField.Name);

    #endregion
}
