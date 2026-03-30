using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestSystemPersistence
{

    #region Fields

    private QuestSystemManager questSystemManager;

    #endregion

    #region Construcor

    public QuestSystemPersistence() 
    { 
        questSystemManager = QuestSystemManager.Instance;
    }

    #endregion

    #region Import

    public void Import(QuestSystemSaveData data)
    {
        foreach(Quest quest in questSystemManager.QuestDict.Values) 
        {
            quest.Import(data.QuestSaveDataDict[quest.QuestId]);
        }
    }

    #endregion

    #region Export

    public QuestSystemSaveData Export()
    {
        return new QuestSystemSaveData
        {
            CurrentMainQuestId = questSystemManager.CurrentMainQuestId,
            CurrentActiveQuestId = questSystemManager.CurrentActiveQuestId,
            QuestSaveDataDict = GetQuestSaveDataDict()
        };
    }

    #endregion

    #region Helpers

    private Dictionary<string, QuestSaveData> GetQuestSaveDataDict() 
    {
        Dictionary<string, QuestSaveData> dict = new Dictionary<string, QuestSaveData>();

        foreach(Quest quest in questSystemManager.QuestDict.Values) 
        {
            dict[quest.QuestId] = quest.Export();
        }

        return dict;
    }

    #endregion

}
