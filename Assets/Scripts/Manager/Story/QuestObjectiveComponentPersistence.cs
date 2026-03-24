using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestObjectiveComponentPersistence
{
    #region Fields

    private QuestObjective questObjective;

    #endregion        

    #region Construcor

    public QuestObjectiveComponentPersistence(
        QuestObjectiveData questObjectiveData, 
        QuestObjective questObjective, 
        QuestObjectiveSaveData questObjectiveSaveData)
    {
        this.questObjective = questObjective;
    }

    #endregion

    #region Import

    public void Import(QuestObjectiveSaveData questObjectiveSaveData)
    {
        if (questObjectiveSaveData.IsCompleted) 
        {
            questObjective.MarkAsCompleted();
        } else 
        {
            questObjective.UpdateProgress(questObjectiveSaveData.CurrentAmount);
        }
    }

    #endregion

    #region Export

    public QuestObjectiveSaveData Export()
    {
        return new QuestObjectiveSaveData
        {
            QuestObjectiveId = questObjective.QuestObjectiveId,
            CurrentAmount = questObjective.CurrentAmount,
            IsCompleted = questObjective.IsCompleted;
        };
    }

    #endregion

    #region Helpers

    #endregion
}
