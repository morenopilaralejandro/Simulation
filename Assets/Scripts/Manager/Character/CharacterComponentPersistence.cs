using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentPersistence : MonoBehaviour
{
    #region Fields

    private Character character;

    #endregion        

    #region LifeCycle

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }

    #endregion

    #region Import

    public void Import(CharacterSaveData characterSaveData)
    {
        CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterSaveData.CharacterId);
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
            LearnedMoves = GetLearnedMovesForSave(),
            EquippedMovesIds = GetEquippedMoveIdsForSave(),

            //avatar
            IsCustomAvatar = false,
            CustomName = null,
            CustomCharacterSize = CharacterSize.S,
            CustomPortraitSize = PortraitSize.S,
            CustomGender = Gender.M,
            CustomElement = Element.Fire,
            CustomPosition = Position.FW,
            CustomBaseStats = null
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

    private List<MoveSaveData> GetLearnedMovesForSave()
    {
        List<MoveSaveData> learnedMoves = new List<MoveSaveData>();
        foreach (Move move in character.LearnedMoves)
            learnedMoves.Add(move.Export());

        return learnedMoves;
    }

    private List<string> GetEquippedMoveIdsForSave()
    {
        List<string> equippedMoveIds = new List<string>();
        foreach (Move move in character.EquippedMoves)
            equippedMoveIds.Add(move.MoveId);

        return equippedMoveIds;
    }

    #endregion
}
