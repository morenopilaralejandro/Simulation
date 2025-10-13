using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentTraining : MonoBehaviour
{
    private Character character;
    public const int MAX_TRAINING_PER_STAT = 50;
    
    [SerializeField] private int baseFreedom;
    [SerializeField] private int trueFreedom;

    public int BaseFreedom => baseFreedom;
    public int TrueFreedom => trueFreedom;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
        this.baseFreedom = characterData.Freedom;
        this.trueFreedom = this.baseFreedom;
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
    }

}
