using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentAIDifficulty
{
    #region Field

    private Character character;

    public AIDifficulty AIDifficulty { get; private set; }

    #endregion

    #region Lifecycle

    public CharacterComponentAIDifficulty(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;
        AIDifficulty = AIDifficulty.Hard;
    }

    #endregion

    #region Logic

    public void ScaleDifficultySystem() 
    {
        ScaleDifficultyByLevel();
        ScaleDifficultyMove();
        ScaleDifficultyWing();
    }

    private void ScaleDifficultyByLevel()
    {
        if(character.Level > 90) 
        {
            AIDifficulty = AIDifficulty.Hard;
        } 
        else if (character.Level < 90 && character.Level > 20)
        {
            AIDifficulty = AIDifficulty.Normal;
        } 
        else 
        {
            AIDifficulty = AIDifficulty.Easy;
        }
    }

    private void ScaleDifficultyMove()
    {
        /*
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                closeDistanceBall = CLOSE_DISTANCE_GK;
                break;
            case AIDifficulty.Normal:
                closeDistanceBall = CLOSE_DISTANCE_DF;
                break;
            case AIDifficulty.Hard:
                closeDistanceBall = CLOSE_DISTANCE_DF;
                break;
            default:
                closeDistanceBall = CLOSE_DISTANCE_OTHER;
                break;
        }
        */
        if(AIDifficulty == AIDifficulty.Hard) 
        {
            character.ForceMaxEvolutionOnEquippedMoves();
        }
    }

    private void ScaleDifficultyWing()
    {
        if(!character.HasWingEquipped) return;
        switch (AIDifficulty)
        {
            /*
            case AIDifficulty.Easy:
                closeDistanceBall = CLOSE_DISTANCE_GK;
                break;
            */
            case AIDifficulty.Normal:
                character.Wing.ForceMaxIndividual();
                break;
            case AIDifficulty.Hard:
                character.Wing.ForceMaxEvolution();
                character.Wing.ForceMaxRefinement();
                character.Wing.ForceMaxIndividual();
                break;
            /*
            default:
                break;
            */
        }
    }

    #endregion
}
