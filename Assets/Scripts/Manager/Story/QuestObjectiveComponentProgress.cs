using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestObjectiveComponentProgress
{
    private QuestObjective questObjective;

    public int RequiredAmount { get; private set; }
    public int CurrentAmount { get; private set; }
    public bool IsCompleted { get; private set; }

    public QuestObjectiveComponentProgress(
        QuestObjectiveData questObjectiveData, 
        QuestObjective questObjective, 
        QuestObjectiveSaveData questObjectiveSaveData)
    {
        this.questObjective = questObjective;

        RequiredAmount = questObjectiveData.RequiredAmount;
        CurrentAmount = 0;
        IsCompleted = false;

        if (questObjectiveSaveData != null)
            questObjective.Import(questObjectiveSaveData);
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

    public void UpdateProgress(int amount)
    {
        CurrentAmount += amount;
        if (CurrentAmount >= RequiredAmount)
            MarkAsCompleted(); 
    }

}
