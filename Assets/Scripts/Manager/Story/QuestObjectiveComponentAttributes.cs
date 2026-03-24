using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestObjectiveComponentAttributes
{
    public string ObjectiveId { get; private set; }
    public ObjectiveType ObjectiveType { get; private set; }
    public string TargetId { get; private set; }
    public int RequiredAmount { get; private set; }
    public bool IsOptional { get; private set; }
    public bool IsHidden { get; private set; }

    public QuestObjectiveComponentAttributes(
        QuestObjectiveData questObjectiveData, 
        QuestObjective questObjective, 
        QuestObjectiveSaveData questObjectiveSaveData)
    {
        ObjectiveId = questObjectiveData.ObjectiveId;
        ObjectiveType = questObjectiveData.ObjectiveType;
        TargetId = questObjectiveData.TargetId;
        RequiredAmount = questObjectiveData.RequiredAmount;
        IsOptional = questObjectiveData.IsOptional;
        IsHidden = questObjectiveData.IsHidden;
    }
}
