using UnityEngine;
using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class CharacterComponentAppearance
{
    #region Fields

    public HairStyle HairStyle { get; private set; }
    public HairColorType HairColorType { get; private set; }
    public EyeColorType EyeColorType { get; private set; }
    public BodyColorType BodyColorType { get; private set; }
    public PortraitSize PortraitSize { get; private set; }
    public string KitId { get; private set; }
    public Variant KitVariant { get; private set; }
    public Role KitRole { get; private set; }

    #endregion

    #region Initialization
    public CharacterComponentAppearance(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        HairStyle = characterData.HairStyle;
        HairColorType = characterData.HairColorType;
        EyeColorType = characterData.EyeColorType;
        BodyColorType = characterData.BodyColorType;
        PortraitSize = characterData.PortraitSize;

        if (characterSaveData != null && characterSaveData.IsCustomAvatar) 
        {
            HairStyle = characterSaveData.CustomHairStyle;
            HairColorType = characterSaveData.CustomHairColorType;
            EyeColorType = characterSaveData.CustomEyeColorType;
            BodyColorType = characterSaveData.CustomBodyColorType;
            PortraitSize = characterSaveData.CustomPortraitSize;
        }
    }

    public CharacterComponentAppearance(NpcData npcData)
    {
        Initialize(npcData);
    }

    public void Initialize(NpcData data)
    {
        HairStyle = data.HairStyle;
        HairColorType = data.HairColorType;
        EyeColorType = data.EyeColorType;
        BodyColorType = data.BodyColorType;
        PortraitSize = data.PortraitSize;
        //pass everyting as data
        //await LoadAsync();
    }

    #endregion

    #region Helpers

    public void SetKitId(Kit kit) => KitId = kit.KitId;
    public void SetKit(Kit kit, Variant variant, Role role) {
        KitId = kit.KitId;
        KitVariant = variant;
        KitRole = role;
    }
    public void SetKit(Team team, Position position) {
        KitId = team.Kit.KitId;
        KitVariant = GetKitVariant(team);
        KitRole = GetKitRole(position);
    }
    public Variant GetKitVariant(Team team) => team?.Variant ?? Variant.Home;
    public Role GetKitRole(Position position) => position == Position.GK ? Role.Keeper : Role.Field;

    #endregion
}
