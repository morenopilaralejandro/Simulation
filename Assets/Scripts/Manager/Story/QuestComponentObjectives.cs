using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestComponentObjectives
{
    private QuestObjective auxObjective;

    private Dictionary<string, QuestObjective> objectives = new Dictionary<string, QuestObjective>();
    public IReadOnlyDictionary<string, QuestObjective> QuestObjectiveDict => objectives;

    public QuestComponentObjectives(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        QuestObjectiveDatabase questObjectiveDatabase = QuestObjectiveDatabase.Instance;
        foreach (string objectiveId in questData.ObjectiveIds)
        {
            var questObjectiveData = questObjectiveDatabase.GetQuestObjectiveData(objectiveId);
            var questObjectiveSaveData = questSaveData?.QuestObjectiveSaveDataDict[objectiveId];
            objectives[objectiveId] = new QuestObjective(questObjectiveData, questObjectiveSaveData);
        }
    }

    public void MarkObjectiveAsCompleted(string objectiveId)
    {
        if(!objectives.TryGetValue(objectiveId, out auxObjective)) return;
        auxObjective.MarkAsCompleted();
    }

    public bool IsObjectiveComplete(string objectiveId)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective))
            return auxObjective.IsCompleted;
        else 
            return false;
    }

    public void UpdateObjectiveProgress(string objectiveId, int amount)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective))
            auxObjective.UpdateProgress(amount);
    }

    public int GetObjectiveProgress(string objectiveId)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective))
            return auxObjective.CurrentAmount;
        else 
            return 0;
    }

    public bool AreAllObjectivesComplete()
    {
        foreach (var kvp in objectives)
        {
            if(!kvp.Value.IsCompleted)
                return false;
        }
        return true;
    }
}
