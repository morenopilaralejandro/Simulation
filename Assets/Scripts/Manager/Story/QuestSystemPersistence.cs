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

    public void Import(QuestSystemSaveData questSystemSaveData)
    {
        foreach(var serializableKeyValue in questSystemSaveData.QuestSaveDataList) 
        {
            QuestSaveData questSaveData = serializableKeyValue.Value;
            questSystemManager.QuestDict[questSaveData.QuestId].Import(questSaveData);
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
            QuestSaveDataList = GetQuestSaveDataList()
        };
    }

    #endregion

    #region Helpers

    private List<SerializableKeyValue<string, QuestSaveData>> GetQuestSaveDataList() 
    {
        var list = new List<SerializableKeyValue<string, QuestSaveData>>(questSystemManager.QuestDict.Count);

        foreach(Quest quest in questSystemManager.QuestDict.Values) 
        {
            list.Add(new SerializableKeyValue<string, QuestSaveData>
            {
                Key = quest.QuestId,
                Value = quest.Export()
            });
        }

        return list;
    }

    #endregion

}
