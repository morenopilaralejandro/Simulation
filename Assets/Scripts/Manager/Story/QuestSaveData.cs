using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class QuestSaveData
{
    public string QuestId;
    public QuestState State;
    public long TimestampStart;
    public long TimestampEnd;
    public Dictionary<string, QuestObjectiveSaveData> QuestObjectiveSaveDataDict;
}
