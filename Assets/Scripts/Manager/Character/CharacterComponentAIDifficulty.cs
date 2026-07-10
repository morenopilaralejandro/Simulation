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
        ScaleDifficultyEquipment();
    }

    #endregion

    #region Difficulty

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

    #endregion

    #region Move

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

    #endregion

    #region Wing

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

    #region Equipment

    private void ScaleDifficultyEquipment()
    {
        if(!character.HasWingEquipped) return;
        switch (AIDifficulty)
        {
            case AIDifficulty.Easy:
                EquiptEasy();
                break;
            case AIDifficulty.Normal:
                EquiptNormal();
                break;
            case AIDifficulty.Hard:
                EquiptHard();
                break;
            /*
            default:
                break;
            */
        }
    }

    private void EquiptEasy()
    {
        switch (character.Position)
        {
            case Position.FW:
                EquiptEquipmentById("item-equipment-00001-spike_fw-01");                
                EquiptEquipmentById("item-equipment-00021-bracelet_fw-01");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00046-cross_evil-01");
                else
                    EquiptEquipmentById("item-equipment-00041-cross_good-01");
                EquiptEquipmentById("item-equipment-00051-pendant_fw-01");
                break;
            case Position.MF:
                EquiptEquipmentById("item-equipment-00006-spike_mf-01");                
                EquiptEquipmentById("item-equipment-00026-bracelet_mf-01");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00046-cross_evil-01");
                else
                    EquiptEquipmentById("item-equipment-00041-cross_good-01");
                EquiptEquipmentById("item-equipment-00056-pendant_mf-01");
                break;
            case Position.DF:
                EquiptEquipmentById("item-equipment-00011-spike_df-01");                
                EquiptEquipmentById("item-equipment-00031-bracelet_df-01");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00046-cross_evil-01");
                else
                    EquiptEquipmentById("item-equipment-00041-cross_good-01");
                EquiptEquipmentById("item-equipment-00061-pendant_df-01");
                break;
            case Position.GK:
                EquiptEquipmentById("item-equipment-00016-spike_gk-01");                
                EquiptEquipmentById("item-equipment-00036-bracelet_gk-01");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00046-cross_evil-01");
                else
                    EquiptEquipmentById("item-equipment-00041-cross_good-01");
                EquiptEquipmentById("item-equipment-00066-gloves_gk-01");
                break;
        } 
    }

    private void EquiptNormal()
    {
        switch (character.Position)
        {
            case Position.FW:
                EquiptEquipmentById("item-equipment-00003-spike_fw-03");                
                EquiptEquipmentById("item-equipment-00023-bracelet_fw-03");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00048-cross_evil-03");
                else
                    EquiptEquipmentById("item-equipment-00043-cross_good-03");
                EquiptEquipmentById("item-equipment-00053-pendant_fw-03");
                break;
            case Position.MF:
                EquiptEquipmentById("item-equipment-00008-spike_mf-03");                
                EquiptEquipmentById("item-equipment-00028-bracelet_mf-03");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00048-cross_evil-03");
                else
                    EquiptEquipmentById("item-equipment-00043-cross_good-03");
                EquiptEquipmentById("item-equipment-00058-pendant_mf-03");
                break;
            case Position.DF:
                EquiptEquipmentById("item-equipment-00013-spike_df-03");                
                EquiptEquipmentById("item-equipment-00033-bracelet_df-03");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00048-cross_evil-03");
                else
                    EquiptEquipmentById("item-equipment-00043-cross_good-03");
                EquiptEquipmentById("item-equipment-00063-pendant_df-03");
                break;
            case Position.GK:
                EquiptEquipmentById("item-equipment-00018-spike_gk-03");                
                EquiptEquipmentById("item-equipment-00038-bracelet_gk-03");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00048-cross_evil-03");
                else
                    EquiptEquipmentById("item-equipment-00043-cross_good-03");
                EquiptEquipmentById("item-equipment-00068-gloves_gk-03");
                break;
        } 
    }

    private void EquiptHard()
    {
        switch (character.Position)
        {
            case Position.FW:
                EquiptEquipmentById("item-equipment-00005-spike_fw-05");                
                EquiptEquipmentById("item-equipment-00025-bracelet_fw-05");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00050-cross_evil-05");
                else
                    EquiptEquipmentById("item-equipment-00045-cross_good-05");
                EquiptEquipmentById("item-equipment-00055-pendant_fw-05");
                break;
            case Position.MF:
                EquiptEquipmentById("item-equipment-00010-spike_mf-05");                
                EquiptEquipmentById("item-equipment-00030-bracelet_mf-05");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00050-cross_evil-05");
                else
                    EquiptEquipmentById("item-equipment-00045-cross_good-05");
                EquiptEquipmentById("item-equipment-00060-pendant_mf-05");
                break;
            case Position.DF:
                EquiptEquipmentById("item-equipment-00015-spike_df-05");                
                EquiptEquipmentById("item-equipment-00035-bracelet_df-05");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00050-cross_evil-05");
                else
                    EquiptEquipmentById("item-equipment-00045-cross_good-05");
                EquiptEquipmentById("item-equipment-00065-pendant_df-05");
                break;
            case Position.GK:
                EquiptEquipmentById("item-equipment-00020-spike_gk-05");                
                EquiptEquipmentById("item-equipment-00040-bracelet_gk-05");
                if (character.Element == Element.Evil)
                    EquiptEquipmentById("item-equipment-00050-cross_evil-05");
                else
                    EquiptEquipmentById("item-equipment-00045-cross_good-05");
                EquiptEquipmentById("item-equipment-00070-gloves_gk-05");
                break;
        } 
    }

    private void EquiptEquipmentById(string id) 
    {
        character.EquipEquipment(ItemFactory.CreateById(id) as ItemEquipment);
    }

    #endregion

}
