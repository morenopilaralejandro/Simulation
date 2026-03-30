using UnityEngine;
using System;
using System.Collections.Generic;

public static class CharacterFactory
{
    public static Character CreateFromSaveData(CharacterSaveData characterSaveData) 
    {
        CharacterData characterData = CharacterDatabase.Instance.GetCharacterData(
            characterSaveData.IsCustomAvatar ? 
                characterSaveData.CustomAvatarId : 
                characterSaveData.CharacterId);

        Character character = new Character(characterData, characterSaveData);
        return character;
    }

    /*
    public static Character Create() 
    {
        no args
        placeholder
    }

    public static Character CreateForBattle() 
    {
        placeholder
    }

    public static Character CreateForWorldPlayer() 
    {
        placeholder
    }

    public static Character CreateForScout()
    {
        use level, scout data etc.
    }

    public static Character CreateForAvatar()
    {
        TODO recieve CharacterAvatarData from the character editor and use it to edit the save data

        create one with out save data
        export the save data
        edit the avatar fields
        create a new one with the edited save data

        
        customAvatarId = "avatar_{element}_{position}"        
        character data get by customAvatarId
        new Character customAvatarId
        save data = character.export
        save data.custom = value
        character. import(save data)

        ------

        old code 

        CharacterSaveData characterSaveData = new CharacterSaveData
        {
            // identity
            CharacterId = customAvatarId,
            CharacterGuid = Guid.NewGuid().ToString(),

            // level
            Level = 1,
            CurrentExp = 0,
            ExpToNextLevel = 1,

            // stats
            CurrentHp = character.GetBattleStat(Stat.Hp),
            CurrentSp = character.GetBattleStat(Stat.Sp),
            TrainedStats = GetTrainedStatsForSave(),

            // training
            CurrentFreedom = character.TrueFreedom,
            TrainingResetCount = character.TrainingResetCount,

            // moves
            LearnedMoves = ExportLearnedMoves(),
            EquippedMovesIds = ExportEquippedMoveIds(),

            //avatar
            IsCustomAvatar = character.IsCustomAvatar,
            CustomAvatarId = character.CustomAvatarId,
            CustomName = character.CustomName,
            CustomCharacterSize = character.CustomCharacterSize,
            CustomGender = character.CustomGender,
            CustomElement = character.CustomElement,
            CustomPosition = character.CustomPosition,
            CustomHairStyle = character.CustomHairStyle,
            CustomHairColorType = character.CustomHairColorType,
            CustomEyeColorType = character.CustomEyeColorType,
            CustomBodyColorType = character.CustomBodyColorType,
            CustomPortraitSize = character.CustomPortraitSize
        };
        return new Character(null, teamSaveData);
    }
    */
}
