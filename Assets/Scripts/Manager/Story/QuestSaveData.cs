using System;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

[Serializable]
public class QuestSaveData
{
    public string QuestId;
    public QuestState State;
    public long TimestampStart;
    public long TimestampEnd;
    public Dictionary<string, QuestObjectiveSaveData> QuestObjectiveSaveDataDict;
}
