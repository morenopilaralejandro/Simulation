using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentPersistance : MonoBehaviour
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
    /*
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
            TrainedStats = stats.GetTrainedStatsForSave(),

            // training
            CurrentFreedom = character.CurrentFreedom,
            TrainingResetCount = character.TrainingResetCount,

            // moves
            learnedMoveIds = moves.GetLearnedMovesForSave(),
            equippedMoveIds = moves.GetEquippedMoveIdsForSave()
        };
    }

    #endregion

    #region Helpers

    #endregion
    */
}
