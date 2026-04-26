using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentPersistence
{
    #region Fields

    private Character character;

    #endregion        

    #region LifeCycle

    public CharacterComponentPersistence(CharacterData characterData, Character character)
    {
        Initialize(characterData, character);
    }

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }

    #endregion

    #region Import

    public void Import(CharacterSaveData characterSaveData)
    {
        CharacterData characterData = CharacterDatabase.Instance.GetCharacterData(
            characterSaveData.IsCustomAvatar ? 
                characterSaveData.CustomAvatarId : 
                characterSaveData.CharacterId);
        character.Initialize(characterData, characterSaveData);
    }

    #endregion

    #region Export

    public CharacterSaveData Export()
    {
        return new CharacterSaveData
        {
            // identity
            CharacterId = character.CharacterId,
            CharacterGuid = character.CharacterGuid,

            // level
            Level = character.Level,
            CurrentExp = character.CurrentExp,
            ExpToNextLevel = character.ExpToNextLevel,

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
    }

    #endregion

    #region Helpers

    private List<CharacterStatSaveData> GetTrainedStatsForSave()
    {
        List<CharacterStatSaveData> trainedStats = new List<CharacterStatSaveData>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            trainedStats.Add(new CharacterStatSaveData
            {
                Stat = stat,
                Val = character.GetTrainedStat(stat)
            });
        }
        return trainedStats;
    }

    private List<MoveSaveData> ExportLearnedMoves()
    {
        List<MoveSaveData> learnedMoves = new List<MoveSaveData>();
        foreach (Move move in character.LearnedMoves)
            learnedMoves.Add(move.Export());

        return learnedMoves;
    }

    private List<string> ExportEquippedMoveIds()
    {
        List<string> equippedMoveIds = new List<string>();
        foreach (Move move in character.EquippedMoves)
            equippedMoveIds.Add(move.MoveId);

        return equippedMoveIds;
    }

    #endregion
}
