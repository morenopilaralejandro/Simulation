using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class QuestObjectiveSaveData
{
    public string QuestObjectiveId;
    public int CurrentAmount => progressComponent.RequiredAmount;
    public bool IsCompleted => progressComponent.RequiredAmount;
}
