using UnityEngine;
using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;
using Aremoreno.Enums.Kit;

public class WingComponentAppearance
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
    public string KitAddress { get; private set; }

    public string PortraitCharacterAddress { get; private set; }
    public string PortraitKitAddress { get; private set; }
    public string HairFrontAddress { get; private set; }
    public string HairBackAddress { get; private set; }

    public Color ColorBody { get; private set; }
    public Color ColorHair { get; private set; }

    #endregion

    #region Initialization

    public WingComponentAppearance(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        /*
        HairStyle = characterData.HairStyle;
        HairColorType = characterData.HairColorType;
        EyeColorType = characterData.EyeColorType;
        BodyColorType = characterData.BodyColorType;
        PortraitSize = characterData.PortraitSize;
        PortraitCharacterAddress = AddressableLoader.GetCharacterPortraitAddress(characterData.CharacterId);

        if (characterSaveData != null && characterSaveData.IsCustomAvatar) 
        {
            HairStyle = characterSaveData.CustomHairStyle;
            HairColorType = characterSaveData.CustomHairColorType;
            EyeColorType = characterSaveData.CustomEyeColorType;
            BodyColorType = characterSaveData.CustomBodyColorType;
            PortraitSize = characterSaveData.CustomPortraitSize;
        }


        HairFrontAddress = AddressableLoader.GetCharacterHairFrontAddress(HairStyle.ToString().ToLower());
        HairBackAddress = AddressableLoader.GetCharacterHairBackAddress(HairStyle.ToString().ToLower());

        ColorBody = ColorManager.GetBodyColor(BodyColorType);
        ColorHair = ColorManager.GetHairColor(HairColorType);
        */
    }

    #endregion

    #region Helpers

    public void SetKitId(Kit kit) 
    { 
        KitId = kit.KitId;
        SetKitAddress();
    }
    public void SetKitAddress() 
    {
        KitAddress =
            AddressableLoader.GetKitBodyAddress(
                KitId,
                KitVariant.ToString().ToLower(),
                KitRole.ToString().ToLower()
            );

        PortraitKitAddress =    
            AddressableLoader.GetKitPortraitAddress(
                KitId,
                KitVariant,
                KitRole,
                PortraitSize
            );
    }
    public void SetKit(Kit kit, Variant variant, Role role) {
        KitId = kit.KitId;
        KitVariant = variant;
        KitRole = role;
        SetKitAddress();
    }
    public void SetKit(Team team, Position position) {
        KitId = team.Kit.KitId;
        KitVariant = GetKitVariant(team);
        KitRole = GetKitRole(position);
        SetKitAddress();
    }
    public Variant GetKitVariant(Team team) => team?.Variant ?? Variant.Home;
    public Role GetKitRole(Position position) => position == Position.GK ? Role.Keeper : Role.Field;

    #endregion
}
