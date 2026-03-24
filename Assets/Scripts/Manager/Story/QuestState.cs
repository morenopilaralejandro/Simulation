using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[System.Serializable]
public class QuestState
{
    public string QuestId { get; private set; }
    public QuestStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CompletionTime { get; set; }

    private Dictionary<string, bool> completedObjectives = new Dictionary<string, bool>();
    private Dictionary<string, int> objectiveProgress = new Dictionary<string, int>();

    public QuestState(string questId)
    {
        QuestId = questId;
        Status = QuestStatus.NotStarted;
    }

    public void CompleteObjective(string objectiveId)
    {
        completedObjectives[objectiveId] = true;
    }

    public bool IsObjectiveComplete(string objectiveId)
    {
        return completedObjectives.TryGetValue(objectiveId, out bool value) && value;
    }

    public void UpdateObjectiveProgress(string objectiveId, int amount)
    {
        if (!objectiveProgress.ContainsKey(objectiveId))
            objectiveProgress[objectiveId] = 0;

        objectiveProgress[objectiveId] += amount;
    }

    public int GetObjectiveProgress(string objectiveId)
    {
        return objectiveProgress.TryGetValue(objectiveId, out int value) ? value : 0;
    }

    public bool AreAllObjectivesComplete(List<QuestObjective> objectives)
    {
        foreach (var obj in objectives)
        {
            if (!IsObjectiveComplete(obj.objectiveId))
                return false;
        }
        return true;
    }

    public QuestStateSaveData ToSaveData()
    {
        return new QuestStateSaveData
        {
            questId = QuestId,
            status = Status,
            startTime = StartTime.ToBinary(),
            completionTime = CompletionTime.ToBinary(),
            completedObjectives = new Dictionary<string, bool>(completedObjectives),
            objectiveProgress = new Dictionary<string, int>(objectiveProgress)
        };
    }

    public static QuestState FromSaveData(QuestStateSaveData data)
    {
        var state = new QuestState(data.questId)
        {
            Status = data.status,
            StartTime = DateTime.FromBinary(data.startTime),
            CompletionTime = DateTime.FromBinary(data.completionTime),
            completedObjectives = new Dictionary<string, bool>(data.completedObjectives),
            objectiveProgress = new Dictionary<string, int>(data.objectiveProgress)
        };
        return state;
    }
}
