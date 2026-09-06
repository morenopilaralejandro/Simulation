using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentTraining
{
    private Character character;
    public const int MAX_TRAINING_PER_STAT = 50;
    public const int TRAINING_POINT_COST = 500;
    
    private int baseFreedom;
    private int trueFreedom;
    private int trainingResetCount;

    public int BaseFreedom => baseFreedom;
    public int TrueFreedom => trueFreedom;
    public int TrainingResetCount => trainingResetCount;

    public CharacterComponentTraining(CharacterData characterData, Character character, CharacterSaveData characterSaveData) 
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData) 
    {
        this.character = character;
        baseFreedom = characterData.Freedom;

        if (characterSaveData != null) 
        {
            trueFreedom = characterSaveData.CurrentFreedom;
            trainingResetCount = characterSaveData.TrainingResetCount;
        } else 
        {
            trueFreedom = baseFreedom;
            trainingResetCount = 0;
        }
    }

    public void TrainStat(Stat stat, int amount)
    {
        if (amount <= 0) return;
        int actualAmount = Mathf.Min(amount, trueFreedom);
        if (actualAmount <= 0) return;
        int current = character.GetTrainedStat(stat);
        int newValue = Mathf.Min(current + actualAmount, MAX_TRAINING_PER_STAT);
        int appliedAmount = newValue - current;
        if (appliedAmount <= 0) return;
        character.ModifyTrainedStat(stat, appliedAmount);
        trueFreedom -= appliedAmount;
    }

    public void UntrainStat(Stat stat, int amount)
    {
        if (amount <= 0) return;
        int current = character.GetTrainedStat(stat);
        int actualAmount = Mathf.Min(amount, current);
        if (actualAmount <= 0) return;
        character.ModifyTrainedStat(stat, -actualAmount);
        trueFreedom += actualAmount;
    }

    public void ApplyTrainingDelta(Stat stat, int delta)
    {
        if (delta > 0)
            TrainStat(stat, delta);
        else if (delta < 0)
            UntrainStat(stat, -delta);
    }

    public bool IsCharacterTrainable(Stat stat)
    {
        return trueFreedom > 0;
    }

    public bool IsStatTrainable(Stat stat)
    {
        return this.character.GetTrainedStat(stat) < MAX_TRAINING_PER_STAT;
    }

    public int GetRemainingTrainingByStat(Stat stat) 
    {
        return MAX_TRAINING_PER_STAT - this.character.GetTrainedStat(stat);
    }

    public void ResetTraining() 
    {
        this.trueFreedom = this.baseFreedom;
        this.character.ResetTrainedStats();
        trainingResetCount++;
    }

}
