using System;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

[Serializable]
public class QuestObjectiveSaveData
{
    public string QuestObjectiveId;
    public int CurrentAmount;
    public bool IsCompleted;
}
