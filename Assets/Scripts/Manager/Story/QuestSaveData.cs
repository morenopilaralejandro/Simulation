using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class QuestSaveData
{
    public string questId;
    public QuestStatus status;
    public long startTime;
    public long completionTime;
    public Dictionary<string, bool> completedObjectives;
    public Dictionary<string, int> objectiveProgress;
}
