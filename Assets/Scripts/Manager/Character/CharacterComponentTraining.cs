using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentTraining
{
    private Character character;
    public const int MAX_TRAINING_PER_STAT = 50;
    
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
        this.character.ModifyTrainedStat(stat, amount);
        trueFreedom--;
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
