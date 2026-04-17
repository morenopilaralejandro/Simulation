using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;

public class CharacterComponentCustomAvatar
{
    public bool IsCustomAvatar { get; private set; }
    public string CustomAvatarId { get; private set; }
    public string CustomName { get; private set; }
    public CharacterSize CustomCharacterSize { get; private set; }
    public Gender CustomGender { get; private set; }
    public Element CustomElement { get; private set; }
    public Position CustomPosition { get; private set; }

    public HairStyle CustomHairStyle { get; private set; }
    public HairColorType CustomHairColorType { get; private set; }
    public EyeColorType CustomEyeColorType { get; private set; }
    public BodyColorType CustomBodyColorType { get; private set; }
    public PortraitSize CustomPortraitSize { get; private set; }

    public CharacterComponentCustomAvatar(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        if(characterSaveData != null)
        {
            IsCustomAvatar = characterSaveData.IsCustomAvatar;
            CustomAvatarId = characterSaveData.CustomAvatarId;
            CustomName = characterSaveData.CustomName;
            CustomCharacterSize = characterSaveData.CustomCharacterSize;
            CustomGender = characterSaveData.CustomGender;
            CustomElement = characterSaveData.CustomElement;
            CustomPosition = characterSaveData.CustomPosition;

            CustomHairStyle = characterSaveData.CustomHairStyle;
            CustomHairColorType = characterSaveData.CustomHairColorType;
            CustomEyeColorType = characterSaveData.CustomEyeColorType;
            CustomBodyColorType = characterSaveData.CustomBodyColorType;
            CustomPortraitSize = characterSaveData.CustomPortraitSize;
        } else
        {
            IsCustomAvatar = false;
            CustomAvatarId = "";
            CustomName = "";
            CustomCharacterSize = CharacterSize.M;
            CustomGender = Gender.M;
            CustomElement = Element.Fire;
            CustomPosition = Position.FW;

            CustomHairStyle = HairStyle.Fade;
            CustomHairColorType = HairColorType.Blonde;
            CustomEyeColorType = EyeColorType.Blue;
            CustomBodyColorType = BodyColorType.Generic;
            CustomPortraitSize = PortraitSize.M;
        }
    }

    //public void SetCustomName(string customName) => CustomName = customName;
    /*
        in order to edit a custom avatar,
        export the character save data
        modify this values
        reinstaciate the character so that it is aplied to all component

        default CustomAvatarId is "avatar_{CustomElement}_{CustomPosition}"
            used to get stat and learnset
    */
}
