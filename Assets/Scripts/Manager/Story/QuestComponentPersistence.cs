using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestComponentPersistence
{
    #region Fields

    private Quest quest;
    private QuestData questData;

    #endregion        

    #region Construcor

    public QuestComponentPersistence(QuestData questData, Quest quest, QuestSaveData questSaveData)
    {
        this.quest = quest;
        this.questData = questData;
    }

    #endregion

    #region Import

    public void Import(QuestSaveData questSaveData)
    {
        quest.Initialize(questData, questSaveData);
    }

    #endregion

    #region Export

    public QuestSaveData Export()
    {
        return new QuestSaveData
        {
            QuestId = quest.QuestId,
            State = quest.State,
            TimestampStart = quest.TimestampStart,
            TimestampEnd = quest.TimestampEnd,
            QuestObjectiveSaveDataDict = GetQuestObjectiveSaveDataDict()
        };
    }

    #endregion

    #region Helpers

    private Dictionary<string, QuestObjectiveSaveData> GetQuestObjectiveSaveDataDict() 
    {
        Dictionary<string, QuestObjectiveSaveData> dict = new Dictionary<string, QuestObjectiveSaveData>();

        foreach(QuestObjective questObjective in quest.QuestObjectiveDict.Values) 
        {
            dict[questObjective.QuestObjectiveId] = questObjective.Export();
        }

        return dict;
    }

    #endregion
}
