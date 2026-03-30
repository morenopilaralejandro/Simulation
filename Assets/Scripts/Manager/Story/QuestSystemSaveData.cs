using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class QuestSystemSaveData
{
    public string CurrentMainQuestId;
    public string CurrentActiveQuestId;
    public Dictionary<string, QuestSaveData> QuestSaveDataDict;
}

