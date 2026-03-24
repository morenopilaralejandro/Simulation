using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class QuestSystemSaveData
{
    public Dictionary<string, QuestSaveData> QuestSaveDataDict;
}

