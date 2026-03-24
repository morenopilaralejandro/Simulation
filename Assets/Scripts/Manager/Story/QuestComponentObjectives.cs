using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestComponentObjectives
{
    private QuestObjective auxObjective;

    private Dictionary<string, QuestObjective> objectives = new Dictionary<string, bool>();
    public IReadOnlyDictionary<string, QuestObjective> QuestObjectiveDict => objectives;

    public QuestComponentObjectives(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        QuestObjectiveDatabase questObjectiveDatabase = QuestObjectiveDatabase.Instance;
        foreach (string objectiveId questData.ObjectiveIds)
        {
            questObjectiveData = questObjectiveDatabase.GetQuestObjectiveData(objectiveId);
            questObjectiveData = questSaveData?.QuestObjectiveSaveDataDict[objectiveId];
            objectives[objectiveId] = new QuestObjective(questObjectiveData, questObjectiveData);
        }
    }

    public void MarkObjectiveAsCompleted(string objectiveId)
    {
        if(!objectives.TryGetValue(objectiveId, out auxObjective)) return;
        auxObjective.MarkAsCompleted();
    }

    public bool IsObjectiveComplete(string objectiveId)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective) 
            auxObjective.IsCompleted;
        else 
            return false;
    }

    public void UpdateObjectiveProgress(string objectiveId, int amount)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective) 
            auxObjective.UpdateProgress(amount);
        else 
            return false;
    }

    public int GetObjectiveProgress(string objectiveId)
    {
        if(objectives.TryGetValue(objectiveId, out auxObjective) 
            auxObjective.CurrentAmount;
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
